using System;
using System.Collections;
using System.Text;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Net;
using Mirage.Command;
using System.Diagnostics;
using Mirage.Communication;
using Mirage.Util;
using System.Threading;
using System.IO;

namespace Mirage.IO
{

    /// <summary>
    /// Manages client factorys and polls them all for readable, writeable and errored clients.
    /// Follows the same calling sequence as a normal IClientFactory instance.
    /// </summary>
    public class ClientManager
    {
        private List<IClientFactory> _factories;
        private ISynchronizedQueue<IClient> _newClients;
        private bool _started = false;
        private BlockingQueue<ClientOperation> workItems;
        protected ISynchronizedQueue<IClient> _internalNewClients;

        protected IList<Thread> threads;
        protected IList<Socket> _sockets;
        protected Hashtable _clientMap;

        /// <summary>
        /// Creates a new instance of the ClientManager
        /// </summary>
        public ClientManager()
        {
            _factories = new List<IClientFactory>();
            workItems = new BlockingQueue<ClientOperation>(15);
            _internalNewClients = new SynchronizedQueue<IClient>();
            threads = new List<Thread>();
            _sockets = new List<Socket>();
            _clientMap = new Hashtable();
        }

        /// <summary>
        /// Add a client factory to be managed by this instance of the client manager.
        /// </summary>
        /// <param name="factory">the factory to be managed</param>
        public void AddFactory(IClientFactory factory)
        {
            _factories.Add(factory);
        }



        /// <summary>
        /// Polls the listening clients to see which ones are ready to read, ready to be written
        /// to and have errors.  These lists can then be access through the ReadableClients,
        /// WritableClients, and ErroredClients properties.
        /// </summary>
        /// <param name="timeout">time to wait for a client event</param>
        /// <returns>true if any clients are ready to be read, written or errored</returns>
        public virtual void Run()
        {
            // start some threads
            for (int i = 0; i < 1; i++)
            {
                Thread t = new Thread(new ThreadStart(ProcessIO));
                t.IsBackground = true;
                t.Start();
                threads.Add(t);
            }

            _sockets.Clear();
            _clientMap.Clear();

            // start the factories
            foreach(IClientFactory factory in _factories) {
                factory.Start();
                _sockets.Add(factory.Socket);
                _clientMap.Add(factory.Socket, factory);
            }

            DateTime startTime;
            TimeSpan elapsed;
            while (true)
            {
                startTime = DateTime.Now;

                RemoveClosedConnections();               
                List<Socket> checkRead = new List<Socket>(_sockets);
                List<Socket> checkWrite = new List<Socket>(_sockets);
                List<Socket> checkError = new List<Socket>(_sockets);

                Socket.Select(checkRead, checkWrite, checkError, 100000);

                foreach (Socket s in checkRead)
                {
                    object client = _clientMap[s];
                    if (client is IClientFactory)
                        workItems.Enqueue(new ClientOperation(client, OpType.Accept));
                    else
                        workItems.Enqueue(new ClientOperation(client, OpType.Read));
                }
                foreach (Socket s in checkWrite)
                {
                    object client = _clientMap[s];
                    workItems.Enqueue(new ClientOperation(client, OpType.Write));
                }
                foreach (Socket s in checkError)
                {
                    object client = _clientMap[s];
                    workItems.Enqueue(new ClientOperation(client, OpType.Error));
                }
                while (workItems.Count > 0)
                    Thread.Sleep(100);

                // add any new clients
                IClient newClient;
                while (_internalNewClients.TryDequeue(out newClient))
                {
                    _sockets.Add(newClient.TcpClient.Client);
                    _clientMap.Add(newClient.TcpClient.Client, newClient);
                    _newClients.Enqueue(newClient);
                }

                // make sure there's some time between polls so we don't burn up all the cpu
                elapsed = DateTime.Now.Subtract(startTime);
                if (elapsed.TotalMilliseconds < 50)
                {
                    Thread.Sleep(50 - (int) elapsed.TotalMilliseconds);
                }
            }
        }

        /// <summary>
        /// Removes and cleans up any closed or errored connections
        /// </summary>
        private void RemoveClosedConnections()
        {
            Queue<Socket> removeItems = new Queue<Socket>();

            foreach (DictionaryEntry entry in _clientMap)
            {
                Socket skey = (Socket) entry.Key;
                IClient client = entry.Value as IClient;
                if (client != null)
                {
                    if (!client.IsOpen)
                    {
                        // close the connection
                        client.Dispose();
                        // remove from the list
                        removeItems.Enqueue(skey);
                    }
                }
            }

            while (removeItems.Count > 0)
            {
                Socket sck = removeItems.Dequeue();
                _clientMap.Remove(sck);
                _sockets.Remove(sck);
            }
        }

        private void ProcessIO()
        {
            while (true)
            {
                ClientOperation op = workItems.Dequeue();  // blocks until something is ready
                try
                {
                    switch (op.Type)
                    {
                        case OpType.Accept:
                            IClient client = ((IClientFactory)op.Client).Accept();
                            _internalNewClients.Enqueue(client);
                            break;
                        case OpType.Read:
                            ((IClient)op.Client).ReadInput();
                            break;
                        case OpType.Write:
                            ((IClient)op.Client).FlushOutput();
                            break;
                        case OpType.Error:
                            ((IClient)op.Client).Close();
                            break;
                    }
                }
                catch (IOException e)
                {
                    if (op.Client is IClient)
                    // error, close the connection, main thread will clean it up
                        ((IClient)op.Client).Close();
                }
            }
        }

        private enum OpType
        {
            Accept,
            Read,
            Write,
            Error
        }

        private struct ClientOperation
        {
            private object _client;
            private OpType _type;

            public ClientOperation(object client, OpType type)
            {
                _client = client;
                _type = type;
            }

            public object Client
            {
                get { return this._client; }
            }

            public OpType Type
            {
                get { return this._type; }
            }
        }



        public void Start()
        {
            Thread t = new Thread(new ThreadStart(Run));
            t.IsBackground = true;
            t.Start();
            _started = true;
        }

        public void Stop() {
            foreach (IClientFactory factory in _factories)
            {
                factory.Stop();
            }
            _started = false;
        }

        public ISynchronizedQueue<IClient> NewClients
        {
            get { return this._newClients; }
            set {
                if (_started)
                {
                    throw new InvalidOperationException("NewClients can not be set while the factory is running");
                }
                this._newClients = value; 
            }
        }
    }




}

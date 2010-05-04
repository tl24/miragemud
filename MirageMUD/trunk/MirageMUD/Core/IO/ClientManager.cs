using System;
using System.Collections;
using System.Text;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Net;
using Mirage.Core.Command;
using System.Diagnostics;
using Mirage.Core.Communication;
using Mirage.Core.Util;
using System.Threading;
using System.IO;
using System.Configuration;
using Castle.Core;

namespace Mirage.Core.IO
{

    /// <summary>
    /// Manages client factorys and polls them all for readable, writeable and errored clients.
    /// Follows the same calling sequence as a normal IClientFactory instance.
    /// </summary>
    [CastleComponent("ClientManager")]
    public class ClientManager
    {
        private List<IClientListener> _listeners;
        private ISynchronizedQueue<IMudClient> _newClients;
        private bool _started = false;
        private BlockingQueue<ClientOperation> workItems;
        private int _maxThreads = 0;
        private ISynchronizedQueue<ITelnetClient> _internalNewClients;

        private IList<Thread> threads;
        private IList<Socket> _sockets;
        private Hashtable _clientMap;

        /// <summary>
        /// Creates a new instance of the ClientManager
        /// </summary>
        public ClientManager(IList<IClientListener> listeners)
            : this(listeners, Environment.ProcessorCount)
        {
        }

        /// <summary>
        /// Creates a new instance of the ClientManager
        /// </summary>
        public ClientManager(IList<IClientListener> listeners, int maxThreads)
        {
            _listeners = new List<IClientListener>(listeners);
            _maxThreads = maxThreads > 0 ? maxThreads : 1;
            workItems = new BlockingQueue<ClientOperation>(15);
            _internalNewClients = new SynchronizedQueue<ITelnetClient>();
            threads = new List<Thread>();
            _sockets = new List<Socket>();
            _clientMap = new Hashtable();
        }

        /// <summary>
        /// Polls the listening clients to see which ones are ready to read, ready to be written
        /// to and have errors.  These lists can then be access through the ReadableClients,
        /// WritableClients, and ErroredClients properties.
        /// </summary>
        /// <param name="timeout">time to wait for a client event</param>
        /// <returns>true if any clients are ready to be read, written or errored</returns>
        protected virtual void Run()
        {
            StartWorkerThreads();
            StartListeners();

            DateTime startTime;
            TimeSpan elapsed;
            while (true)
            {
                startTime = DateTime.Now;

                RemoveClosedConnections();
                PollForIO();

                // add any new clients
                AddNewClients();

                // make sure there's some time between polls so we don't burn up all the cpu
                elapsed = DateTime.Now.Subtract(startTime);
                if (elapsed.TotalMilliseconds < 50)
                {
                    Thread.Sleep(50 - (int)elapsed.TotalMilliseconds);
                }
            }
        }

        /// <summary>
        /// Adds any new clients found during the Poll phase
        /// </summary>
        private void AddNewClients()
        {
            ITelnetClient newClient;
            while (_internalNewClients.TryDequeue(out newClient))
            {
                _sockets.Add(newClient.TcpClient.Client);
                _clientMap.Add(newClient.TcpClient.Client, newClient);
                _newClients.Enqueue(newClient);
            }
        }

        /// <summary>
        /// Polls the sockets to see if any have any pending activity and the queues the
        /// appropriate action
        /// </summary>
        private void PollForIO()
        {
            List<Socket> checkRead = new List<Socket>(_sockets);
            List<Socket> checkWrite = new List<Socket>(_sockets);
            List<Socket> checkError = new List<Socket>(_sockets);

            Socket.Select(checkRead, checkWrite, checkError, 100000);

            foreach (Socket s in checkRead)
            {
                object client = _clientMap[s];
                if (client is ClientListener)
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
        }

        private void StartListeners()
        {
            _sockets.Clear();
            _clientMap.Clear();

            // start the factories
            foreach (ClientListener listener in _listeners)
            {
                listener.Start();
                _sockets.Add(listener.Socket);
                _clientMap.Add(listener.Socket, listener);
            }
        }

        private void StartWorkerThreads()
        {
            for (int i = 0; i < 1; i++)
            {
                Thread t = new Thread(new ThreadStart(ProcessIO));
                t.Name = "IOWorker" + i;
                t.IsBackground = true;
                t.Start();
                threads.Add(t);
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
                Socket skey = (Socket)entry.Key;
                ITelnetClient client = entry.Value as ITelnetClient;
                if (client != null)
                {
                    if (!client.IsOpen)
                    {
                        client.FlushOutput();
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
                            ITelnetClient client = ((ClientListener)op.Client).Accept();
                            _internalNewClients.Enqueue(client);
                            break;
                        case OpType.Read:
                            ((ITelnetClient)op.Client).ReadInput();
                            break;
                        case OpType.Write:
                            ((ITelnetClient)op.Client).FlushOutput();
                            break;
                        case OpType.Error:
                            ((ITelnetClient)op.Client).Close();
                            break;
                    }
                }
                catch (IOException e)
                {
                    if (op.Client is ITelnetClient)
                        // error, close the connection, main thread will clean it up
                        ((ITelnetClient)op.Client).Close();
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
            t.Name = "IOManager";
            t.IsBackground = true;
            t.Start();
            _started = true;
        }

        public void Stop()
        {
            
            foreach (ClientListener listener in _listeners)
            {
                listener.Stop();
            }
            _started = false;
        }

        public ISynchronizedQueue<IMudClient> NewClients
        {
            get { return this._newClients; }
            set
            {
                if (_started)
                {
                    throw new InvalidOperationException("NewClients can not be set while the factory is running");
                }
                this._newClients = value;
            }
        }
    }
}

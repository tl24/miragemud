using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using Mirage.Core.Collections;
using Castle.Core;
using System.Collections.Concurrent;

namespace Mirage.Core.IO.Net
{

    /// <summary>
    /// Manages client factorys and polls them all for readable, writeable and errored clients.
    /// Follows the same calling sequence as a normal IClientFactory instance.
    /// </summary>
    public class ConnectionManager
    {
        private List<IConnectionListener> _listeners;
        private BlockingCollection<IConnection> _newClients;
        private bool _started = false;
        private BlockingCollection<ClientOperation> _workItems;
        private int _maxThreads = 0;
        private ConcurrentQueue<SocketConnection> _internalNewClients;

        private IList<Thread> threads;
        private IList<Socket> _sockets;
        private Hashtable _clientMap;
        private int _processCount;
        private ManualResetEvent _allProcessedEvent = new ManualResetEvent(false);

        /// <summary>
        /// Creates a new instance of the ClientManager
        /// </summary>
        public ConnectionManager(IList<IConnectionListener> listeners)
            : this(listeners, Environment.ProcessorCount)
        {
        }

        /// <summary>
        /// Creates a new instance of the ClientManager
        /// </summary>
        public ConnectionManager(IList<IConnectionListener> listeners, int maxThreads)
        {
            _listeners = new List<IConnectionListener>(listeners);
            _maxThreads = maxThreads > 0 ? maxThreads : 1;
            _workItems = new BlockingCollection<ClientOperation>(15);
            _internalNewClients = new ConcurrentQueue<SocketConnection>();
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
            SocketConnection newClient;
            while (_internalNewClients.TryDequeue(out newClient))
            {
                _sockets.Add(newClient.TcpClient.Client);
                _clientMap.Add(newClient.TcpClient.Client, newClient);
                _newClients.Add(newClient);
            }
        }

        /// <summary>
        /// Polls the sockets to see if any have any pending activity and the queues the
        /// appropriate action
        /// </summary>
        private void PollForIO()
        {
            //TODO: Use Socket.DuplicateAndClose for copyover support
            List<Socket> checkRead = new List<Socket>(_sockets);
            List<Socket> checkWrite = new List<Socket>(_sockets);
            List<Socket> checkError = new List<Socket>(_sockets);

            Socket.Select(checkRead, checkWrite, checkError, 100000);
            
            List<ClientOperation> ops = new List<ClientOperation>(checkRead.Count + checkWrite.Count + checkError.Count);
            foreach (Socket s in checkRead)
            {
                object client = _clientMap[s];
                if (client is IConnectionListener)
                    ops.Add(new ClientOperation(client, OpType.Accept));
                else
                    ops.Add(new ClientOperation(client, OpType.Read));
            }
            foreach (Socket s in checkWrite)
            {
                object client = _clientMap[s];
                ops.Add(new ClientOperation(client, OpType.Write));
            }
            foreach (Socket s in checkError)
            {
                object client = _clientMap[s];
                ops.Add(new ClientOperation(client, OpType.Error));
            }

            _processCount = ops.Count;
            if (_processCount > 0)
            {
                _allProcessedEvent.Reset();
                foreach (ClientOperation op in ops)
                    _workItems.Add(op);

                _allProcessedEvent.WaitOne();
            }
        }

        private void StartListeners()
        {
            _sockets.Clear();
            _clientMap.Clear();

            // start the factories
            foreach (IConnectionListener listener in _listeners)
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
                SocketConnection client = entry.Value as SocketConnection;
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
            // blocks until something is ready
            foreach (var op in _workItems.GetConsumingEnumerable())
            {
                try
                {
                    switch (op.Type)
                    {
                        case OpType.Accept:
                            SocketConnection client = ((IConnectionListener)op.Client).Accept();
                            _internalNewClients.Enqueue(client);
                            break;
                        case OpType.Read:
                            ((SocketConnection)op.Client).ReadInput();
                            break;
                        case OpType.Write:
                            ((SocketConnection)op.Client).FlushOutput();
                            break;
                        case OpType.Error:
                            ((SocketConnection)op.Client).Close();
                            break;
                    }
                }
                catch (IOException)
                {
                    if (op.Client is SocketConnection)
                        // error, close the connection, main thread will clean it up
                        ((SocketConnection)op.Client).Close();
                }
                int count = Interlocked.Decrement(ref _processCount);
                if (count == 0)
                {
                    _allProcessedEvent.Set();
                }
            }
        }

        private enum OpType
        {
            Accept = 1,
            Read = 2,
            Write = 4,
            Error = 8
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
            
            foreach (IConnectionListener listener in _listeners)
            {
                listener.Stop();
            }
            _started = false;
        }

        public BlockingCollection<IConnection> NewClients
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

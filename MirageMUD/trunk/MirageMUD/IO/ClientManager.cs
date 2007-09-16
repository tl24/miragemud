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
using System.Configuration;

namespace Mirage.IO
{

    /// <summary>
    /// Manages client factorys and polls them all for readable, writeable and errored clients.
    /// Follows the same calling sequence as a normal IClientFactory instance.
    /// </summary>
    public class ClientManager
    {
        private List<ClientListener> _listeners;
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
            _listeners = new List<ClientListener>();
            workItems = new BlockingQueue<ClientOperation>(15);
            _internalNewClients = new SynchronizedQueue<IClient>();
            threads = new List<Thread>();
            _sockets = new List<Socket>();
            _clientMap = new Hashtable();
        }

        /// <summary>
        /// Add a client listener to be managed by this instance of the client manager.
        /// </summary>
        /// <param name="factory">the listener to be managed</param>
        public void AddListener(ClientListener listener)
        {
            _listeners.Add(listener);
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
            foreach (ClientListener listener in _listeners)
            {
                listener.Start();
                _sockets.Add(listener.Socket);
                _clientMap.Add(listener.Socket, listener);
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
                    Thread.Sleep(50 - (int)elapsed.TotalMilliseconds);
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
                Socket skey = (Socket)entry.Key;
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
                            IClient client = ((ClientListener)op.Client).Accept();
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

        public void Stop()
        {
            foreach (ClientListener listener in _listeners)
            {
                listener.Stop();
            }
            _started = false;
        }

        public ISynchronizedQueue<IClient> NewClients
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

        public void Configure()
        {
            ClientManagerConfiguration section = (ClientManagerConfiguration)ConfigurationManager.GetSection("ClientManager");
            foreach (ListenerConfiguration listener in section.Listeners)
            {                
                if (string.IsNullOrEmpty(listener.Host))
                    AddListener(new ClientListener(listener.Port, (IClientFactory) Activator.CreateInstance(Type.GetType(listener.ClientFactory))));
                else {
                    IPAddress[] addresses = System.Net.Dns.GetHostAddresses(listener.Host);
                    if (addresses.Length > 0)
                    {
                        AddListener(new ClientListener(
                            new IPEndPoint(addresses[0], listener.Port),
                            (IClientFactory) Activator.CreateInstance(Type.GetType(listener.ClientFactory))));
                    }
                    else
                    {
                        throw new ArgumentException("Invalid host name: " + listener.Host, "host");
                    }
                }
            }
        }
    }

    public class ClientManagerConfiguration : ConfigurationSection
    {
        [ConfigurationProperty("Listeners", IsDefaultCollection = true)]
        public ListenersCollectionConfiguration Listeners
        {
            get { return this["Listeners"] as ListenersCollectionConfiguration; }
        }
    }

    public class ListenersCollectionConfiguration : ConfigurationElementCollection
    {
        public ListenerConfiguration this[int index]
        {
            get
            {
                return base.BaseGet(index) as ListenerConfiguration;
            }
            set
            {
                if (base.BaseGet(index) != null)
                {
                    base.BaseRemoveAt(index);
                }
                this.BaseAdd(index, value);
            }
        }

        public ListenerConfiguration this[string key]
        {
            get
            {
                return base.BaseGet(key) as ListenerConfiguration;
            }
            set
            {
                if (base.BaseGet(key) != null)
                {
                    base.BaseRemove(key);
                }
                this.BaseAdd(value);
            }
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new ListenerConfiguration();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((ListenerConfiguration)element).Key;
        }
    }

    public class ListenerConfiguration : ConfigurationElement
    {
        [ConfigurationProperty("host")]
        public string Host
        {
            get { return base["host"] as string; }
        }

        [ConfigurationProperty("port", IsRequired=true)]
        public int Port
        {
            get { return int.Parse(base["port"].ToString()); }
        }

        [ConfigurationProperty("client-factory", IsRequired=true)]
        public string ClientFactory
        {
            get { return base["client-factory"] as string; }
        }

        [ConfigurationProperty("key", IsKey=true)]
        public string Key
        {
            get {
                string host = Host;
                if (string.IsNullOrEmpty(host))
                    host = "localhost";

                return host + ":" + Port;
            }
        }
    }
}

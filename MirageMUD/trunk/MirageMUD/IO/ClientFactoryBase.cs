using System;
using System.Collections.Generic;
using System.Text;
using log4net;
using System.Net.Sockets;
using System.Net;

namespace Mirage.IO
{
    /// <summary>
    /// Base class for ClientFactory implementations.  Provides most of the framework for a client factory.
    /// Child factory implementations should only need to implement InitConnection.
    /// </summary>
    public abstract class ClientFactoryBase : IClientFactory
    {
        private ILog log;

        protected IList<Socket> _sockets;
        protected IDictionary<Socket, IClient> _clientMap;
        protected TcpListener _listener;

        protected IList<IClient> _readable;
        protected IList<IClient> _writable;
        protected IList<IClient> _errored;

        public ClientFactoryBase(IPEndPoint listeningEndPoint, ILog log)
        {
            this.log = log;
            _listener = new TcpListener(listeningEndPoint);
            _listener.Start();
            log.Info(this.GetType().Name + " listening at address " + _listener.LocalEndpoint.ToString());
            _sockets = new List<Socket>();
            _clientMap = new Dictionary<Socket, IClient>();
            _readable = new List<IClient>();
            _writable = new List<IClient>();
            _errored = new List<IClient>();

        }

        public ClientFactoryBase(int port, ILog log)
            : this(new IPEndPoint(IPAddress.Loopback, port), log)
        {
        }
        #region IClientFactory Members

        /// <summary>
        /// Polls the listening clients to see which ones are ready to read, ready to be written
        /// to and have errors.  These lists can then be access through the ReadableClients,
        /// WritableClients, and ErroredClients properties.
        /// </summary>
        /// <param name="timeout">time to wait for a client event</param>
        /// <returns>true if any clients are ready to be read, written or errored</returns>
        public virtual bool Poll(int timeout)
        {
            int i = 0;
            while (i < _sockets.Count)
            {
                Socket sck = _sockets[i];
                if (!sck.Connected)
                {
                    _clientMap.Remove(sck);
                    _sockets.RemoveAt(i);
                }
                else
                {
                    _clientMap[sck].CommandRead = false;
                    i++;
                }
            }

            _readable.Clear();
            _writable.Clear();
            _errored.Clear();

            while (_listener.Pending())
            {
                TcpClient client = _listener.AcceptTcpClient();
                InitConnection(client);
            }

            if (_sockets.Count > 0)
            {
                List<Socket> checkRead = new List<Socket>(_sockets);
                List<Socket> checkWrite = new List<Socket>(_sockets);
                List<Socket> checkError = new List<Socket>(_sockets);

                Socket.Select(checkRead, checkWrite, checkError, timeout);

                _readable = checkRead.ConvertAll<IClient>(new Converter<Socket, IClient>(this.Converter));
                _writable = checkWrite.ConvertAll<IClient>(new Converter<Socket, IClient>(this.Converter));
                _errored = checkError.ConvertAll<IClient>(new Converter<Socket, IClient>(this.Converter));
            }

            return (_readable.Count > 0 || _writable.Count > 0 || _errored.Count > 0);
        }

        private IClient Converter(Socket s)
        {
            return _clientMap[s];
        }
        /// <summary>
        ///     Initialize a new connection
        /// </summary>
        /// <param name="client"></param>
        protected void InitConnection(TcpClient client)
        {
            Socket newSocket = client.Client;
            _sockets.Add(newSocket);
            log.Info("Connection from " + client.Client.RemoteEndPoint.ToString());
            IClient mudClient = CreateClient(client);
            _clientMap[newSocket] = mudClient;

        }

        protected abstract IClient CreateClient(TcpClient client);

        public virtual IList<IClient> ReadableClients
        {
            get { return _readable; }
        }

        public virtual IList<IClient> WritableClients
        {
            get { return _writable; }
        }

        public virtual IList<IClient> ErroredClients
        {
            get { return _errored; }
        }

        /// <summary>
        /// Shuts down the listener and all connected client created by the factory
        /// </summary>
        public virtual void Shutdown()
        {
            _listener.Stop();
            foreach (IClient client in _clientMap.Values)
            {
                client.Close();
            }
            _clientMap.Clear();
            _sockets.Clear();
            _readable.Clear();
            _writable.Clear();
            _errored.Clear();
        }

        /// <summary>
        /// Remove the client from this factory and close it if its not closed
        /// </summary>
        /// <param name="client">the client to remove</param>
        public virtual void Remove(IClient client)
        {
            if (_clientMap.ContainsKey(client.TcpClient.Client))
            {
                if (client.IsOpen)
                {
                    client.Close();
                }
                _clientMap.Remove(client.TcpClient.Client);
                _sockets.Remove(client.TcpClient.Client);
            }
        }
        #endregion
    }

}

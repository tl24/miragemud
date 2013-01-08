using System;
using System.Net;
using System.Net.Sockets;
using Castle.Core.Logging;

namespace Mirage.Core.IO.Net
{
    /// <summary>
    /// Base class for ClientFactory implementations.  Provides most of the framework for a client factory.
    /// Child factory implementations should only need to implement InitConnection.
    /// </summary>
    public abstract class ConnectionListenerBase : IConnectionListener, IDisposable
    {
        protected TcpListener _listener;

        public ConnectionListenerBase(string host, int port)
            : this(GetEndpoint(host, port))
        {
        }

        public ConnectionListenerBase(IPEndPoint listeningEndPoint)
        {
            _listener = new TcpListener(listeningEndPoint);
        }

        public ConnectionListenerBase(int port)
        {            
            _listener = new TcpListener(IPAddress.Any, port);
        }

        /// <summary>
        /// Gets the IP endpoint from host and port
        /// </summary>
        /// <param name="host">host name</param>
        /// <param name="port">listening port</param>
        /// <returns>endpoint</returns>
        private static IPEndPoint GetEndpoint(string host, int port)
        {
            IPAddress[] addresses = System.Net.Dns.GetHostAddresses(host);
            if (addresses.Length > 0)
                return new IPEndPoint(addresses[0], port);
            else
                throw new ArgumentException("Invalid host name: " + host, "host");
        }

        #region Logger
        private ILogger logger = NullLogger.Instance;
        public ILogger Logger
        {
            get { return logger; }
            set { logger = value; }
        }
        #endregion

        /// <summary>
        /// Determines if there are connections waiting to be read
        /// </summary>
        /// <returns></returns>
        public bool Pending()
        {
            return _listener.Pending();
        }

        /// <summary>
        /// Accepts a socket from the listener and creates a client object from it and
        /// returns it
        /// </summary>
        /// <returns>new client object</returns>
        public SocketConnection Accept()
        {
            TcpClient client = _listener.AcceptTcpClient();
            Socket newSocket = client.Client;
            Logger.Info("Connection from " + client.Client.RemoteEndPoint.ToString());
            SocketConnection telnetClient = CreateClient(client);
            //telnetClient.Initialize();
            return telnetClient;
        }

        protected abstract SocketConnection CreateClient(TcpClient client);

        /// <summary>
        /// Starts this listener listening for connections
        /// </summary>
        public void Start()
        {
            _listener.Start();
            Logger.Info("listening at address " + _listener.LocalEndpoint.ToString());
        }

        /// <summary>
        /// Shuts down the listener
        /// </summary>
        public void Stop()
        {
            Logger.Info("stopped listening at address " + _listener.LocalEndpoint.ToString());
            _listener.Stop();
        }

        public Socket Socket
        {
            get { return _listener.Server; }
        }

        #region IDisposable Members

        public void Dispose()
        {
            if (_listener != null)
                _listener.Stop();
            _listener = null;
        }

        #endregion
    }

}

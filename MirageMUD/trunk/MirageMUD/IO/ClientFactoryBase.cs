using System;
using System.Collections.Generic;
using System.Text;
using log4net;
using System.Net.Sockets;
using System.Net;
using Mirage.Util;
using System.Threading;
using System.IO;

namespace Mirage.IO
{
    /// <summary>
    /// Base class for ClientFactory implementations.  Provides most of the framework for a client factory.
    /// Child factory implementations should only need to implement InitConnection.
    /// </summary>
    public abstract class ClientFactoryBase : IClientFactory
    {
        private ILog log;

        protected TcpListener _listener;

        public ClientFactoryBase(IPEndPoint listeningEndPoint, ILog log)
        {
            this.log = log;
            _listener = new TcpListener(listeningEndPoint);
        }

        public ClientFactoryBase(int port, ILog log)
            : this(new IPEndPoint(IPAddress.Loopback, port), log)
        {
        }

        public bool Pending()
        {
            return _listener.Pending();
        }

        /// <summary>
        ///     Initialize a new connection
        /// </summary>
        /// <param name="client"></param>
        public IClient Accept()
        {
            TcpClient client = _listener.AcceptTcpClient();
            Socket newSocket = client.Client;
            log.Info("Connection from " + client.Client.RemoteEndPoint.ToString());
            return CreateClient(client);            
        }

        protected abstract IClient CreateClient(TcpClient client);

        public void Start()
        {
            _listener.Start();
            log.Info(this.GetType().Name + " listening at address " + _listener.LocalEndpoint.ToString());
        }

        /// <summary>
        /// Shuts down the listener and all connected client created by the factory
        /// </summary>
        public void Stop()
        {
            _listener.Stop();
        }

        public Socket Socket
        {
            get { return _listener.Server; }
        }
    }

}

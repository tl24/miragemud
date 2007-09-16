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
    public class ClientListener
    {
        private static ILog log = LogManager.GetLogger(typeof(ClientListener));
        private IClientFactory clientFactory;

        protected TcpListener _listener;

        public ClientListener(IPEndPoint listeningEndPoint, IClientFactory clientFactory)
        {
            _listener = new TcpListener(listeningEndPoint);
            this.clientFactory = clientFactory;
        }

        public ClientListener(int port, IClientFactory clientFactory)
        {
            _listener = new TcpListener(port);
            this.clientFactory = clientFactory;
        }

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
        public IClient Accept()
        {
            TcpClient client = _listener.AcceptTcpClient();
            Socket newSocket = client.Client;
            log.Info("Connection from " + client.Client.RemoteEndPoint.ToString());
            return CreateClient(client);            
        }

        private IClient CreateClient(TcpClient client)
        {
            return clientFactory.CreateClient(client);
        }

        /// <summary>
        /// Starts this listener listening for connections
        /// </summary>
        public void Start()
        {
            _listener.Start();
            log.Info(clientFactory.GetType().Name + " listening at address " + _listener.LocalEndpoint.ToString());
        }

        /// <summary>
        /// Shuts down the listener
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

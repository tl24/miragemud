using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.Diagnostics;
using Shoop.Command;
using Shoop.Communication;

namespace Shoop.IO
{
    public class TextClientFactory : IClientFactory
    {
        private IList<Socket> _sockets;
        private IDictionary<Socket, IClient> _clientMap;
        private TcpListener _listener;

        private IList<IClient> _readable;
        private IList<IClient> _writable;
        private IList<IClient> _errored;

        public TextClientFactory(IPEndPoint listeningEndPoint)
        {
            _listener = new TcpListener(listeningEndPoint);
            _listener.Start();
            Trace.WriteLine("Listening at address " + _listener.LocalEndpoint.ToString(), "Server");
            _sockets = new List<Socket>();
            _clientMap = new Dictionary<Socket, IClient>();
            _readable = new List<IClient>();
            _writable = new List<IClient>();
            _errored = new List<IClient>();

        }

        public TextClientFactory(int port)
            : this(new IPEndPoint(IPAddress.Loopback, port))
        {
        }
        #region IClientFactory Members

        public bool Poll(int timeout)
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
        private void InitConnection(TcpClient client)
        {
            Socket newSocket = client.Client;
            _sockets.Add(newSocket);
            IClient mudClient = new TextClient();
            mudClient.Open(client);
            _clientMap[newSocket] = mudClient;

            mudClient.StateHandler = new LoginStateHandler(mudClient);
            Trace.WriteLine("Connection from " + client.Client.RemoteEndPoint.ToString(), "Server");

            mudClient.StateHandler.HandleInput(null);
        }

        public IList<IClient> ReadableClients
        {
            get { return _readable; }
        }

        public IList<IClient> WritableClients
        {
            get { return _writable; }
        }

        public IList<IClient> ErroredClients
        {
            get { return _errored; }
        }

        public void Shutdown()
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

        public void Remove(IClient client)
        {
            TextClient textClient = client as TextClient;
            if (textClient != null)
            {
                if (textClient.IsOpen)
                {
                    textClient.Close();
                }
                _clientMap.Remove(textClient.TcpClient.Client);
                _sockets.Remove(textClient.TcpClient.Client);
            }
        }
        #endregion
    }
}

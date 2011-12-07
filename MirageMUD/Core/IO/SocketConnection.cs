using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net.Sockets;

namespace Mirage.Core.IO
{
    public abstract class SocketConnection : IConnection, IDisposable
    {
        /// <summary>
        /// closed flag, 0 = open, 1 = closed
        /// </summary>
        protected int _closed;

        /// <summary>
        ///     A reference to the tcp client (socket) that this description
        /// reads and writes from
        /// </summary>
        protected TcpClient _client;

        /// <summary>
        ///     Create a client to read and write to the given
        /// tcp client (Socket)
        /// </summary>
        /// <param name="client"> the client to read and write from</param>
        protected SocketConnection(TcpClient client)
        {
            this._client = client;
        }

        #region Logger
        private Castle.Core.Logging.ILogger logger = Castle.Core.Logging.NullLogger.Instance;
        public Castle.Core.Logging.ILogger Logger
        {
            get { return logger; }
            set { logger = value; }
        }
        #endregion

        /// <summary>
        /// Indicates if this client is still open or connected
        /// </summary>
        public bool IsOpen
        {
            get { return _closed == 0 && _client.Connected; }
        }

        /// <summary>
        ///     Closes the underlying connection
        /// </summary>
        public void Close()
        {
            Interlocked.Exchange(ref _closed, 1);
        }

        /// <summary>
        /// Closes the client connection
        /// </summary>
        public virtual void Dispose()
        {
            string remote = _client.Client.RemoteEndPoint.ToString();
            _client.Close();
            Logger.Info("Client connection closed: " + remote);
        }

        /// <summary>
        /// Gets the underlying TcpClient socket
        /// </summary>
        public TcpClient TcpClient
        {
            get { return _client; }
        }

        /// <summary>
        /// Gets the local ip address.
        /// </summary>
        public string Address
        {
            get { return TcpClient.Client.LocalEndPoint.ToString(); }
        }

        public abstract void ReadInput();

        public abstract void FlushOutput();
    }
}

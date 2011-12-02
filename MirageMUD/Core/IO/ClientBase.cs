using System;
using System.Collections.Generic;
using System.Text;
using Mirage.Core.Communication;
using System.Net.Sockets;
using System.IO;
using Mirage.Core.Data;
using Mirage.Core.Util;
using Mirage.Core.Command;
using log4net;
using log4net.Repository.Hierarchy;
using System.Threading;

namespace Mirage.Core.IO
{
    /// <summary>
    ///     Handles Server for a player
    /// </summary>
    public abstract class ClientBase : ITelnetClient
    {
        /// <summary>
        /// closed flag, 0 = open, 1 = closed
        /// </summary>
        protected int _closed;

        /// <summary>
        /// State Handler for this client.  If this is present it takes
        /// precedence over the command interpreter.
        /// </summary>
        protected ILoginInputHandler _loginHandler;

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
        public ClientBase(TcpClient client)
        {
            this._client = client;
        }

        /// <summary>
        ///     The stage of connection that this descriptor is at
        /// </summary>
        public virtual ConnectedState State { get; set; }

        /// <summary>
        ///     The player object attached to this descriptor
        /// </summary>
        public virtual IPlayer Player { get; set; }

        /// <summary>
        ///     state machine handler for the client
        /// </summary>
        public virtual ILoginInputHandler LoginHandler
        {
            get { return _loginHandler; }
            set { _loginHandler = value; }
        }

        /// <summary>
        /// Read from the socket and populate the input queue
        /// </summary>
        public abstract void ReadInput();

        /// <summary>
        /// Initializes the Client
        /// </summary>
        public virtual void Initialize()
        {
        }

        /// <summary>
        /// Process the input from the connection and Execute a command
        /// </summary>
        public abstract void ProcessInput();

        /// <summary>
        ///     Indicates that a Command was read this cycle
        /// </summary>
        public virtual bool CommandRead { get; set; }

        /// <summary>
        /// Returns true if the client had a message written to its queue during this execution loop
        /// </summary>
        /// <returns></returns>
        public virtual bool OutputWritten { get; set; }

        #region Logger
        private Castle.Core.Logging.ILogger logger = Castle.Core.Logging.NullLogger.Instance;
        public Castle.Core.Logging.ILogger Logger
        {
            get { return logger; }
            set { logger = value; }
        }
        #endregion

        /// <summary>
        ///     Process the output waiting in the output buffer.  This
        /// Data will be sent to the socket.
        /// </summary>
        public abstract void FlushOutput();

        /// <summary>
        /// Write a message to the client's output buffer.  The message will not be sent to the
        /// client until FlushOutput is called.
        /// </summary>
        /// <param name="message">The message to write</param>
        public abstract void Write(IMessage message);

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
            State = ConnectedState.Disconnected;
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
    }
}

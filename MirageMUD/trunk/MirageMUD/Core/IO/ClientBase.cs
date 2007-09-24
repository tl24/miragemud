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
    public abstract class ClientBase : IClient
    {

        /// <summary>
        /// closed flag, 0 = open, 1 = closed
        /// </summary>
        protected int _closed;

        protected static ILog log = LogManager.GetLogger(typeof(ClientBase));
        /// <summary>
        ///     The player object attached to this descriptor
        /// </summary>
        protected Player _player;

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
        ///     The stage of connection that this descriptor is at
        /// </summary>
        protected ConnectedState _state;

        /// <summary>
        ///     Indicates that a Command was read this cycle
        /// </summary>
        protected bool _commandRead;

        /// <summary>
        /// Indicates that output was written this cycle
        /// </summary>
        protected bool _outputWritten;
        /// <summary>
        /// The client factory that created this client
        /// </summary>
        protected IClientFactory _clientFactory;

        /// <summary>
        ///     Create a descriptor to read and write to the given
        /// tcp client (Socket)
        /// </summary>
        /// <param name="client"> the client to read and write from</param>
        public virtual void Open(TcpClient client)
        {
            this._client = client;
        }

        /// <summary>
        ///     The stage of connection that this descriptor is at
        /// </summary>
        public virtual ConnectedState State
        {
            get { return _state; }
            set { _state = value; }
        }

        /// <summary>
        ///     The player object attached to this descriptor
        /// </summary>
        public virtual Player Player
        {
            get { return _player; }
            set { _player = value; }
        }

        /// <summary>
        ///     state machine handler for the client
        /// </summary>
        public virtual ILoginInputHandler LoginHandler
        {
            get { return _loginHandler; }
            set { _loginHandler = value; }
        }

        public abstract void ReadInput();


        /// <summary>
        /// Process the input from the connection and Execute a command
        /// </summary>
        public abstract void ProcessInput();

        /// <summary>
        ///     Indicates that a Command was read this cycle
        /// </summary>
        public virtual bool CommandRead
        {
            get { return _commandRead; }
            set { _commandRead = value; }
        }

        public virtual bool OutputWritten
        {
            get { return _outputWritten; }
            set { _outputWritten = value; }
        }

        /// <summary>
        /// Writes a prompt to the client
        /// </summary>
        public void WritePrompt()
        {
            if (Player != null && State == ConnectedState.Playing)
            {
                string clientName = Player.Title;
                Write(new StringMessage(MessageType.Prompt, "DefaultPrompt", clientName + ">> "));
            }
        }

        /// <summary>
        ///     Process the output waiting in the output buffer.  This
        /// Data will be sent to the socket.
        /// </summary>
        public abstract void FlushOutput();

        public abstract void Write(IMessage message);

        /// <summary>
        /// Indicates if this client is still open or connected
        /// </summary>
        public bool IsOpen
        {
            get { return _closed == 0; }
        }

        /// <summary>
        ///     Closes the underlying connection
        /// </summary>
        public void Close()
        {
            Interlocked.Exchange(ref _closed, 1);
        }

        public virtual void Dispose()
        {
            string remote = _client.Client.RemoteEndPoint.ToString();
            _client.Close();
            _state = ConnectedState.Disconnected;
            log.Info("Client connection closed: " + remote);
        }

        public IClientFactory ClientFactory
        {
            get { return _clientFactory; }
            set { _clientFactory = value; }
        }

        public TcpClient TcpClient
        {
            get { return _client; }
        }

        public ILog Logger
        {
            get { return log; }
        }
    }
}

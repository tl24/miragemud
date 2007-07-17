using System;
using System.Collections.Generic;
using System.Text;
using Mirage.Communication;
using System.Net.Sockets;
using System.IO;
using Mirage.Data;
using Mirage.Util;
using Mirage.Command;

namespace Mirage.IO
{
    /// <summary>
    ///     Handles Server for a player
    /// </summary>
    public abstract class ClientBase : IClient
    {
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
        ///     The lines that are waiting to be written to the socket
        /// </summary>
        protected Queue<Message> outputQueue;

        /// <summary>
        ///     The stage of connection that this descriptor is at
        /// </summary>
        protected ConnectedState _state;

        /// <summary>
        ///     Indicates that a Command was read this cycle
        /// </summary>
        protected bool _commandRead;

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
            outputQueue = new Queue<Message>();
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

        /// <summary>
        /// Write the specified text to the descriptors output buffer. 
        /// </summary>
        public void Write(Message message)
        {
            outputQueue.Enqueue(message);
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

        /// <summary>
        /// Checks to see if there is any output waiting to be written to the client
        /// </summary>
        /// <returns></returns>
        public bool HasOutput()
        {
            return outputQueue.Count > 0;
        }

        /// <summary>
        /// Indicates if this client is still open or connected
        /// </summary>
        public bool IsOpen
        {
            get { return _client.Connected; }
        }

        /// <summary>
        ///     Closes the underlying connection
        /// </summary>
        public virtual void Close()
        {
            FlushOutput();
            _client.Close();
            _state = ConnectedState.Disconnected;
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
    }
}

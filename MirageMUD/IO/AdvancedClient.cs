using System;
using System.Collections.Generic;
using System.Text;
using Mirage.Data;
using Mirage.Util;
using System.Net.Sockets;
using System.IO;

namespace Mirage.IO
{
    /// <summary>
    /// Mud client class that can accept and send objects and other complex types
    /// more advanced than strings
    /// </summary>
    public class AdvancedClient : IClient
    {
        /// <summary>
        ///     The player object attached to this descriptor
        /// </summary>
        private Player _player;

        /// <summary>
        /// State Handler for this client.  If this is present it takes
        /// precedence over the command interpreter.
        /// </summary>
        private AbstractStateMachine _stateHandler;

        /// <summary>
        ///     A reference to the tcp client (socket) that this description
        /// reads and writes from
        /// </summary>
        private TcpClient _client;

        /// <summary>
        ///     A reader for the tcp client's stream
        /// </summary>
        private StreamReader reader;

        /// <summary>
        ///     A writer for the tcp client's stream
        /// </summary>
        private StreamWriter writer;

        #region IClient Members

        public void Open(System.Net.Sockets.TcpClient client)
        {
            this._client = client;
            NetworkStream stm = _client.GetStream();
            reader = new StreamReader(stm);
            writer = new StreamWriter(stm);
        }

        public void Close()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void ProcessInput()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void Write(Mirage.Communication.Message message)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void WritePrompt()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void FlushOutput()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public bool HasOutput()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public bool CommandRead
        {
            get
            {
                throw new Exception("The method or operation is not implemented.");
            }
            set
            {
                throw new Exception("The method or operation is not implemented.");
            }
        }

        public ConnectedState State
        {
            get
            {
                throw new Exception("The method or operation is not implemented.");
            }
            set
            {
                throw new Exception("The method or operation is not implemented.");
            }
        }

        public Mirage.Data.Player Player
        {
            get
            {
                throw new Exception("The method or operation is not implemented.");
            }
            set
            {
                throw new Exception("The method or operation is not implemented.");
            }
        }

        public Mirage.Util.AbstractStateMachine StateHandler
        {
            get
            {
                throw new Exception("The method or operation is not implemented.");
            }
            set
            {
                throw new Exception("The method or operation is not implemented.");
            }
        }

        public IClientFactory ClientFactory
        {
            get
            {
                throw new Exception("The method or operation is not implemented.");
            }
            set
            {
                throw new Exception("The method or operation is not implemented.");
            }
        }

        #endregion
    }
}

using System.Collections.Generic;
using Mirage.Game.Command;
using Mirage.Game.Communication;
using Mirage.Game.IO.Net;
using Mirage.Game.World;
using Mirage.Core.IO.Net;
using Mirage.Core.Messaging;

namespace NUnitTests
{
    public class MockClient : IClient
    {
        private bool _isOpen = true;
        private ILoginInputHandler _loginHandler;
        private IPlayer _player;
        private ConnectedState _state = ConnectedState.Playing;
        private bool _commandRead;
        private bool _outputWritten;
        private List<IMessage> _messages = new List<IMessage>();

        public void Initialize()
        {
        }

        public void Close()
        {
            IsOpen = false;
        }

        public void ProcessInput()
        {
            CommandRead = true;
        }

        public void Write(IMessage message)
        {
            _messages.Add(message);
            OutputWritten = true;
        }

        public bool OutputWritten
        {
            get { return _outputWritten; }
            set { _outputWritten = value; }
        }

        public bool CommandRead
        {
            get { return _commandRead; }
            set { _commandRead = true; }
        }

        public ConnectedState State
        {
            get { return _state; }
            set { _state = value; }
        }

        public Mirage.Game.World.IPlayer Player
        {
            get { return _player; }
            set { _player = value; }
        }

        public ILoginInputHandler LoginHandler
        {
            get { return _loginHandler; }
            set { _loginHandler = value; }
        }

        public bool IsOpen
        {
            get { return _isOpen; }
            set { _isOpen = value; }
        }

        public string Address
        {
            get { return this.GetType().FullName; }
        }

        public List<IMessage> Messages
        {
            get { return this._messages; }
        }


    }
}

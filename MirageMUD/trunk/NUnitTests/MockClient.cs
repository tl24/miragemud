using System;
using System.Collections.Generic;
using System.Text;
using Mirage.Core.IO;
using Mirage.Core.Command;
using Mirage.Core.Data;
using Mirage.Core.Communication;

namespace NUnitTests
{
    public class MockClient : IMudClient
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

        public void WritePrompt()
        {
            
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

        public Mirage.Core.Data.IPlayer Player
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

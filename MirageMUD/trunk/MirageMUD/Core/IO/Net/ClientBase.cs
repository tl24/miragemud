using Mirage.Core.IO.Net;
using Mirage.Core.Messaging;

namespace Mirage.Core.IO.Net
{
    public abstract class ClientBase<TClientState> : IClient<TClientState> where TClientState : new()
    {
        private IConnection _connection;

        public ClientBase(IConnection connection)
        {
            _connection = connection;
            ClientState = new TClientState();
        }

        public void Close()
        {
            _connection.Close();
        }

        public abstract void ProcessInput();

        public abstract void Write(IMessage message);

        public bool OutputWritten { get; set; }

        public bool CommandRead { get; set; }

        public bool IsOpen
        {
            get { return _connection.IsOpen; }
        }

        public string Address
        {
            get { return _connection.Address; }
        }

        public TClientState ClientState { get; private set; }
    }
}

using Mirage.Game.Command;
using Mirage.Game.Communication;
using Mirage.Game.World;
using Mirage.IO.Net;
using Mirage.Core.Messaging;

namespace Mirage.Game.IO.Net
{
    public abstract class ConnectionAdapterBase : IConnectionAdapter
    {
        private IConnection _connection;

        public ConnectionAdapterBase(IConnection connection)
        {
            _connection = connection;
        }

        public void Close()
        {
            _connection.Close();
        }

        public abstract void ProcessInput();

        public abstract void Write(IMessage message);

        public bool OutputWritten { get; set; }

        public bool CommandRead { get; set; }

        public ConnectedState State { get; set; }

        public IPlayer Player { get; set; }

        public bool IsOpen
        {
            get { return _connection.IsOpen; }
        }

        public string Address
        {
            get { return _connection.Address; }
        }

        #region IConnectionAdapter Members


        public ILoginInputHandler LoginHandler { get; set; }

        #endregion
    }
}

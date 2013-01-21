using System;
using Mirage.Game.Command;
using Mirage.Core.IO.Net;

namespace Mirage.Game.IO.Net
{
    /// <summary>
    /// Temporary implementation of ConnectionAdapterFactory until we switch it to castle
    /// </summary>
    public class ClientFactory : IClientFactory
    {
        public IClient<ClientPlayerState> CreateConnectionAdapter(IConnection connection)
        {
            if (connection is TextConnection)
            {
                var adapter = new TextClient((TextConnection)connection);
                adapter.ClientState.LoginHandler = new TextLoginHandler(adapter);
                adapter.ClientState.LoginHandler.HandleInput(null);
                return adapter;
            }
            else if (connection is AdvancedConnection)
            {
                // for now, use reflection to get around assembly reference
                var adapter = new AdvancedClient((AdvancedConnection)connection);
                adapter.ClientState.LoginHandler = new AdvancedLoginHandler(adapter);
                adapter.ClientState.LoginHandler.HandleInput(null);
                return adapter;
            }
            throw new Exception("Unknown connection type: " + connection.GetType());
        }
    }
}

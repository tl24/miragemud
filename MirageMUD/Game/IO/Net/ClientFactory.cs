using System;
using Mirage.Game.Command;
using Mirage.IO.Net;

namespace Mirage.Game.IO.Net
{
    /// <summary>
    /// Temporary implementation of ConnectionAdapterFactory until we switch it to castle
    /// </summary>
    public class ClientFactory : IClientFactory
    {
        public IClient CreateConnectionAdapter(IConnection connection)
        {
            if (connection is TextConnection)
            {
                var adapter = new TextClient((TextConnection)connection);
                adapter.LoginHandler = new TextLoginHandler(adapter);
                adapter.LoginHandler.HandleInput(null);
                return adapter;
            }
            else if (connection is AdvancedConnection)
            {
                // for now, use reflection to get around assembly reference
                var adapter = new AdvancedClient((AdvancedConnection)connection);
                adapter.LoginHandler = new AdvancedLoginHandler(adapter);
                adapter.LoginHandler.HandleInput(null);
                return adapter;
            }
            throw new Exception("Unknown connection type: " + connection.GetType());
        }
    }
}

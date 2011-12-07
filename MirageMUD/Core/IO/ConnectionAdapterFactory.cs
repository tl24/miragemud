using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mirage.Core.Command;

namespace Mirage.Core.IO
{
    /// <summary>
    /// Temporary implementation of ConnectionAdapterFactory until we switch it to castle
    /// </summary>
    public class ConnectionAdapterFactory : IConnectionAdapterFactory
    {
        public IConnectionAdapter CreateConnectionAdapter(IConnection connection)
        {
            if (connection is TextConnection)
            {
                // for now, use reflection to get around assembly reference
                var adapter = new TextConnectionAdapter((TextConnection)connection);
                adapter.LoginHandler = (ILoginInputHandler) Activator.CreateInstance(Type.GetType("Mirage.Stock.IO.TextLoginStateHandler, Mirage.Stock"), adapter);
                adapter.LoginHandler.HandleInput(null);
                return adapter;
            }
            else if (connection is AdvancedConnection)
            {
                // for now, use reflection to get around assembly reference
                var adapter = new AdvancedConnectionAdapter((AdvancedConnection)connection);
                adapter.LoginHandler = (ILoginInputHandler)Activator.CreateInstance(Type.GetType("Mirage.Stock.IO.GuiLoginHandler, Mirage.Stock"), adapter);
                adapter.LoginHandler.HandleInput(null);
                return adapter;
            }
            //TODO: handle gui connection
            throw new Exception("Unknown connection type: " + connection.GetType());
        }
    }
}

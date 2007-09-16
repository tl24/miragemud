using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using log4net;
using Mirage.Command;

namespace Mirage.IO
{
    /// <summary>
    /// Creates a client that is capable of sending complex messages and commands
    /// containing objects and data structures as well as text to/from a socket.
    /// </summary>
    public class GuiClientFactory : IClientFactory
    {
        public IClient CreateClient(TcpClient client)
        {
            IClient mudClient = new GuiClient();
            mudClient.Open(client);
            mudClient.LoginHandler = new GuiLoginHandler(mudClient);
            mudClient.LoginHandler.HandleInput(null);
            return mudClient;
        }
    }
}

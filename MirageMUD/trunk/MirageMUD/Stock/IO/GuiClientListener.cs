using System;
using System.Collections.Generic;
using System.Text;
using Mirage.Core.IO;
using System.Net.Sockets;
using System.Net;

namespace Mirage.Stock.IO
{
    /// <summary>
    /// Creates a client that is capable of sending complex messages and commands
    /// containing objects and data structures as well as text to/from a socket.
    /// </summary>
    public class GuiClientListener : ClientListener
    {
        public GuiClientListener(string host, int port)
            : base(host, port)
        {
        }

        public GuiClientListener(IPEndPoint listeningEndPoint)
            : base(listeningEndPoint)
        {
        }

        public GuiClientListener(int port) : base(port)
        {
        }

        protected override ITelnetClient CreateClient(TcpClient client)
        {
            ITelnetClient mudClient = new GuiClient(client);
            mudClient.LoginHandler = new GuiLoginHandler(mudClient);
            mudClient.LoginHandler.HandleInput(null);
            return mudClient;
        }
    }
}

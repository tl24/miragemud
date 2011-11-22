using System;
using System.Collections.Generic;
using System.Text;
using Mirage.Core.IO;
using System.Net.Sockets;
using System.Net;
using Mirage.Core.Data;

namespace Mirage.Stock.IO
{
    /// <summary>
    /// Creates a client that can read/write text-based commands
    /// and messages from a socket
    /// </summary>
    public class TextClientListener : ClientListener
    {
        public TextClientListener(string host, int port)
            : base(host, port)
        {
        }

        public TextClientListener(IPEndPoint listeningEndPoint)
            : base(listeningEndPoint)
        {
        }

        public TextClientListener(int port)
            : base(port)
        {
        }
        /// <summary>
        ///     creates a new client from the TcpClient
        /// </summary>
        /// <param name="client"></param>
        protected override ITelnetClient CreateClient(TcpClient client)
        {
            ITelnetClient mudClient = MudFactory.GetObject<TextClient>(new {client = client});
            //ITelnetClient mudClient = new TextClient(client);
            mudClient.LoginHandler = new TextLoginStateHandler(mudClient);
            mudClient.LoginHandler.HandleInput(null);
            return mudClient;
        }
    }
}

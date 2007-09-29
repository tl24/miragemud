using System;
using System.Collections.Generic;
using System.Text;
using Mirage.Core.IO;
using System.Net.Sockets;

namespace Mirage.Stock.IO
{
    /// <summary>
    /// Creates a client that can read/write text-based commands
    /// and messages from a socket
    /// </summary>
    public class TextClientFactory : IClientFactory
    {
        /// <summary>
        ///     creates a new client from the TcpClient
        /// </summary>
        /// <param name="client"></param>
        public IClient CreateClient(TcpClient client)
        {
            IClient mudClient = new TextClient();
            mudClient.Open(client);
            mudClient.LoginHandler = new TextLoginStateHandler(mudClient);
            mudClient.LoginHandler.HandleInput(null);
            return mudClient;
        }
    }
}

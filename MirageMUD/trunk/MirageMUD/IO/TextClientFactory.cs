using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.Diagnostics;
using Mirage.Command;
using Mirage.Communication;
using log4net;

namespace Mirage.IO
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

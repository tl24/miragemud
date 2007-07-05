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
    public class TextClientFactory : ClientFactoryBase
    {
        private static ILog log = LogManager.GetLogger(typeof(TextClientFactory));

        public TextClientFactory(IPEndPoint listeningEndPoint) : base(listeningEndPoint, log)
        {
        }

        public TextClientFactory(int port) : base(port, log)
        {
        }

        #region IClientFactory Members

        /// <summary>
        ///     creates a new client from the TcpClient
        /// </summary>
        /// <param name="client"></param>
        protected override IClient CreateClient(TcpClient client)
        {
            IClient mudClient = new TextClient();
            mudClient.Open(client);
            mudClient.StateHandler = new LoginStateHandler(mudClient);
            mudClient.StateHandler.HandleInput(null);
            return mudClient;
        }


        
        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using log4net;
using Mirage.Command;

namespace Mirage.IO
{
    public class AdvancedClientFactory : ClientFactoryBase
    {
        private static ILog log = LogManager.GetLogger(typeof(AdvancedClientFactory));

        public AdvancedClientFactory(IPEndPoint listeningEndPoint) : base(listeningEndPoint, log) {
        }

        public AdvancedClientFactory(int port)
            : base(port, log)
        {
        }


        protected override IClient CreateClient(TcpClient client)
        {
            IClient mudClient = new AdvancedClient();
            mudClient.Open(client);
            mudClient.StateHandler = new TextLoginStateHandler(mudClient);
            mudClient.StateHandler.HandleInput(null);
            return mudClient;
        }
    }
}

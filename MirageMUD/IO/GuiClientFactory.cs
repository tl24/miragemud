using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using log4net;
using Mirage.Command;

namespace Mirage.IO
{
    public class GuiClientFactory : ClientFactoryBase
    {
        private static ILog log = LogManager.GetLogger(typeof(GuiClientFactory));

        public GuiClientFactory(IPEndPoint listeningEndPoint) : base(listeningEndPoint, log) {
        }

        public GuiClientFactory(int port)
            : base(port, log)
        {
        }


        protected override IClient CreateClient(TcpClient client)
        {
            IClient mudClient = new GuiClient();
            mudClient.Open(client);
            mudClient.LoginHandler = new GuiLoginHandler(mudClient);
            mudClient.LoginHandler.HandleInput(null);
            return mudClient;
        }
    }
}

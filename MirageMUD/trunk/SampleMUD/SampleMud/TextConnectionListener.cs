using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mirage.Core.IO.Net;
using System.Net.Sockets;

namespace SampleMud
{
    public class TextConnectionListener : ConnectionListenerBase
    {
        public TextConnectionListener(int port)
            : base(port)
        {            
        }

        protected override SocketConnection CreateClient(TcpClient client)
        {
            return new TextConnection(client);
        }
    }
}

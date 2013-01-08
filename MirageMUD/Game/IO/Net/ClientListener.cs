using System.Net;
using System.Net.Sockets;
using Mirage.Game.World;
using Mirage.Core.IO.Net;

namespace Mirage.Game.IO.Net
{
    public class ClientListener<T> : ConnectionListenerBase where T : SocketConnection
    {

        public ClientListener(string host, int port)
            : base(host, port)
        {
        }

        public ClientListener(IPEndPoint listeningEndPoint) : base(listeningEndPoint)
        {
        }

        public ClientListener(int port) : base(port)
        {            
        }

        protected override SocketConnection CreateClient(TcpClient client)
        {
            T mudClient = MudFactory.GetObject<T>(new { client = client });
            return mudClient;
        }
    }
}

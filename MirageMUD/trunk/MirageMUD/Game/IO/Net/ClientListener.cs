using System.Net;
using System.Net.Sockets;
using Mirage.Game.World;
using Mirage.IO.Net;

namespace Mirage.Game.IO.Net
{
    public class ClientListener<T> : ClientListenerBase where T : SocketConnection
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

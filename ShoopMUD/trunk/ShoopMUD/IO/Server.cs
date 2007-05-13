using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.Collections;
using System.Diagnostics;
using Shoop.Data;
using Shoop.Command;
using Shoop.Communication;

namespace Shoop.IO
{
    class Server
    {
        /// <summary>
        ///     The network port being listened on
        /// </summary>
        private int _port;

        /// <summary>
        ///     Shutdown flag to cause the Server loop to stop
        /// </summary>
        private bool _shutdown;

        /// <summary>
        ///     The socket connections
        /// </summary>
        private IList sockets;

        /// <summary>
        ///     A mapping from sockets to their descriptors
        /// </summary>
        private Dictionary<Socket, IClient> descMap;

        public Server(int port)
        {
            _port = port;
            Shutdown = true;
            sockets = new ArrayList();
            descMap = new Dictionary<Socket, IClient>();
        }

        public bool Shutdown {
            get { return _shutdown; }
            set { _shutdown = value; }
        }

        /// <summary>
        ///     Starts the main processing loop that listens for socket
        /// connections and then reads and writes from those that are
        /// ready.  This method will block indefinitely until the
        /// shutdown flag is set to true.
        /// </summary>
        public void Run()
        {
            _shutdown = false;
            TcpListener listener = new TcpListener(System.Net.IPAddress.Loopback ,_port);
            listener.Start();
            Trace.WriteLine("Listening at address " + listener.LocalEndpoint.ToString(), "Server");
            Socket server = listener.Server;

            sockets.Add(server);
            GlobalLists globalLists = GlobalLists.GetInstance();

            while(!_shutdown) {
                int i = 0;
                while (i < sockets.Count)
                {
                    Socket sck = (Socket) sockets[i];
                    if (sck != server && !sck.Connected)
                    {
                        descMap.Remove(sck);
                        sockets.RemoveAt(i);
                    }
                    else
                    {
                        i++;
                    }
                }

                IList readList = new ArrayList(sockets);
                IList writeList = new ArrayList(sockets);
                IList errorList = new ArrayList(sockets);
            
                Socket.Select(readList, writeList, errorList, 100);

                IClient desc;
                foreach (Socket errSocket in errorList) {
                    desc = GetClient(errSocket);
                    if (desc.Player != null)
                    {
                        if (desc.State == ConnectedState.Playing)
                        {
                            Player.Save(desc.Player);
                            globalLists.Players.Remove(desc.Player);
                        }
                    }
                    desc.Close();
                    descMap.Remove(errSocket);
                    sockets.Remove(errSocket);
                }

                foreach (Socket reader in readList) {
                    if (reader == server) {
                        TcpClient client = listener.AcceptTcpClient();
                        InitConnection(client);
                    } else {
                        ReadSocket(reader);
                    }
                }

                foreach (Socket writer in writeList) {
                    WriteSocket(writer);
                }
            }
        }

        /// <summary>
        ///     Initialize a new connection
        /// </summary>
        /// <param name="client"></param>
        private void InitConnection(TcpClient client)
        {
            Socket newSocket = client.Client;
            sockets.Add(newSocket);
            IClient newDesc = new TextClient();
            newDesc.Open(client);
            descMap[newSocket] = newDesc;
            newDesc.Nanny = Nanny.getInstance(newDesc);
            Trace.WriteLine("Connection from " + client.Client.RemoteEndPoint.ToString(), "Server");
	
            newDesc.Write(new StringMessage(MessageType.Information, "Welcome", "Welcome to the mud\n\r"));
            newDesc.Write(new StringMessage(MessageType.Prompt, "Nanny.Name", "Enter Name: "));
        }

        private void WriteSocket(Socket writer)
        {
            IClient desc = GetClient(writer);
            desc.FlushOutput();
        }

        private void ReadSocket(Socket reader)
        {
            IClient desc = GetClient(reader);
            string input = desc.Read();

            if (input != null)
            {
                if (desc.Player != null)
                {
                    Interpreter.executeCommand(desc.Player, input);
                }
                else
                {
                    desc.Nanny.handleInput(input);
                }
            }
        }

        private IClient GetClient(Socket connection) {
            return descMap[connection];
        }
    }
}

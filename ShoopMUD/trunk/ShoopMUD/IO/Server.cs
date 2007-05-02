using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.Collections;
using System.Diagnostics;
using Shoop.Data;
using Shoop.Command;

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
        private Dictionary<Socket, Descriptor> descMap;

        public Server(int port)
        {
            _port = port;
            shutdown = true;
            sockets = new ArrayList();
            descMap = new Dictionary<Socket, Descriptor>();
        }

        public bool shutdown {
            get { return _shutdown; }
            set { _shutdown = value; }
        }

        /// <summary>
        ///     Starts the main processing loop that listens for socket
        /// connections and then reads and writes from those that are
        /// ready.  This method will block indefinitely until the
        /// shutdown flag is set to true.
        /// </summary>
        public void run()
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

                Descriptor desc;
                foreach (Socket errSocket in errorList) {
                    desc = getDescriptor(errSocket);
                    if (desc.player != null)
                    {
                        if (desc.state == ConnectedState.Playing)
                        {
                            Player.Save(desc.player);
                            globalLists.Players.Remove(desc.player);
                        }
                    }
                    desc.close();
                    descMap.Remove(errSocket);
                    sockets.Remove(errSocket);
                }

                foreach (Socket reader in readList) {
                    if (reader == server) {
                        TcpClient client = listener.AcceptTcpClient();
                        initConnection(client);
                    } else {
                        readSocket(reader);
                    }
                }

                foreach (Socket writer in writeList) {
                    writeSocket(writer);
                }
            }
        }

        /// <summary>
        ///     Initialize a new connection
        /// </summary>
        /// <param name="client"></param>
        private void initConnection(TcpClient client)
        {
            Socket newSocket = client.Client;
            sockets.Add(newSocket);
            Descriptor newDesc = new Descriptor(client);
            descMap[newSocket] = newDesc;
            newDesc.nanny = Nanny.getInstance(newDesc);
            Trace.WriteLine("Connection from " + client.Client.RemoteEndPoint.ToString(), "Server");
	
            newDesc.writeToBuffer("Welcome to the mud\n\r");
            newDesc.writeToBuffer("Enter Name: ");
        }

        private void writeSocket(Socket writer)
        {
            Descriptor desc = getDescriptor(writer);
            desc.processOutputBuffer();
        }

        private void readSocket(Socket reader)
        {
            Descriptor desc = getDescriptor(reader);
            if (desc.read())
            {
                desc.readFromBuffer();
                if (desc.player != null)
                {
                    Interpreter.executeCommand(desc.player, desc.inputLine);
                }
                else
                {
                    desc.nanny.handleInput(desc.inputLine);
                }
                desc.inputLine = null;
            }
        }

        private Descriptor getDescriptor(Socket connection) {
            return descMap[connection];
        }
    }
}

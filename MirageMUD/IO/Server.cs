using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.Collections;
using System.Diagnostics;
using Shoop.Data;
using Shoop.Command;
using Shoop.Communication;
using System.Threading;
using log4net;

namespace Shoop.IO
{
    class Server
    {
        private static ILog logger = LogManager.GetLogger(typeof(Server));

        /// <summary>
        ///     The network port being listened on
        /// </summary>
        private int _port;

        /// <summary>
        ///     Shutdown flag to cause the Server loop to stop
        /// </summary>
        private bool _shutdown;

        public Server(int port)
        {
            _port = port;
            Shutdown = true;
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
            ClientManager manager = new ClientManager();
            manager.AddFactory(new TextClientFactory(_port));

            GlobalLists globalLists = GlobalLists.GetInstance();

            DateTime lastTime = DateTime.Now;
            DateTime currentTime = DateTime.Now;
            TimeSpan delta = new TimeSpan();
            int loopCount = 0;

            //TODO: Read this from config
            int PulsePerSecond = 4;

            while(!_shutdown) {
                loopCount++;
                if (manager.Poll(100))
                {
                    foreach (IClient client in manager.ErroredClients)
                    {
                        if (client.Player != null)
                        {
                            if (client.State == ConnectedState.Playing)
                            {
                                Player.Save(client.Player);
                                globalLists.Players.Remove(client.Player);
                            }
                        }
                        client.Close();
                        client.ClientFactory.Remove(client);
                    }

                    foreach (IClient client in manager.ReadableClients)
                    {
                        ReadClient(client);
                    }

                    foreach (IClient client in manager.WritableClients)
                    {
                        WriteClient(client);
                    }
                }

                currentTime = DateTime.Now;
	            delta = lastTime + TimeSpan.FromSeconds(1.0d/PulsePerSecond) - currentTime;
	            if (delta.Ticks > 0) {
	                //Thread.sleep($timedelta);
                    Thread.Sleep(delta);
	            }
	            lastTime = currentTime;

            }
        }

        private void WriteClient(IClient client)
        {
            if ((client.CommandRead || client.HasOutput()))
            {
                WritePrompt(client);
            }
            client.FlushOutput();
        }

        private void ReadClient(IClient client)
        {
            string input = client.Read();

            if (input != null)
            {
                if (client.StateHandler != null) {
                    client.StateHandler.HandleInput(input);
                } else {
                    Interpreter.executeCommand(client.Player, input);
                }
            }
        }

        private void WritePrompt(IClient client)
        {
            //Client specific??
            
            if (client.Player != null && client.State == ConnectedState.Playing) {
                string clientName = client.Player.Title;
                client.Write(new StringMessage(MessageType.Prompt, "DefaultPrompt", clientName + ">> "));
            }
        }

    }
}

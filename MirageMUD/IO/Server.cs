using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.Collections;
using System.Diagnostics;
using Mirage.Data;
using Mirage.Command;
using Mirage.Communication;
using System.Threading;
using log4net;
using Mirage.Util;

namespace Mirage.IO
{
    class Server
    {
        private static ILog logger = LogManager.GetLogger(typeof(Server));

        /// <summary>
        ///     The network port being listened on
        /// </summary>
        private int _port;

        /// <summary>
        ///     Stop flag to cause the Server loop to stop
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
            logger.Info("Starting up");
            ClientManager manager = new ClientManager();
            manager.AddFactory(new TextClientFactory(_port));
            manager.AddFactory(new GuiClientFactory(_port + 1));
            GlobalLists globalLists = GlobalLists.GetInstance();
            List<IClient> NannyClients = new List<IClient>();
            // These are the new connections waiting to be put in the nanny list
            BlockingQueue<IClient> NannyQueue = new BlockingQueue<IClient>(15);

            DateTime lastTime = DateTime.Now;
            DateTime currentTime = DateTime.Now;
            TimeSpan delta = new TimeSpan();
            int loopCount = 0;

            //TODO: Read this from config
            int PulsePerSecond = 4;

            manager.NewClients = NannyQueue;
            manager.Start();

            while(!_shutdown) {
                loopCount++;
                IClient newClient;

                while (NannyQueue.TryDequeue(out newClient))
                {
                    NannyClients.Add(newClient);
                }

                for (int i = NannyClients.Count - 1; i >= 0; i--)
                {
                    NannyClients[i].ProcessInput();
                    if (NannyClients[i].Player != null && NannyClients[i].State == ConnectedState.Playing)
                    {
                        // graduated...remove from the list
                        NannyClients[i].WritePrompt();
                        NannyClients.RemoveAt(i);
                        
                    }
                }

                Queue<Player> removePlayers = new Queue<Player>();

                // reset state
                foreach (Player player in globalLists.Players)
                {
                    player.Client.CommandRead = false;
                    player.Client.OutputWritten = false;
                    if (!player.Client.IsOpen)
                    {
                        player.save();
                        removePlayers.Enqueue(player);
                    }
                }

                while (removePlayers.Count > 0)
                {
                    removePlayers.Dequeue().FirePlayerEvent(Player.PlayerEventType.Quiting);
                    //globalLists.RemovePlayer(removePlayers.Dequeue());                    
                }

                foreach (Player player in globalLists.Players)
                {
                    player.Client.ProcessInput();
                }

                foreach (Player player in globalLists.Players)
                {
                    if (player.Client.CommandRead || player.Client.OutputWritten)
                        player.Client.WritePrompt();

                }

                currentTime = DateTime.Now;
	            delta = lastTime + TimeSpan.FromSeconds(1.0d/PulsePerSecond) - currentTime;
	            if (delta.Ticks > 0) {
	                //Thread.sleep($timedelta);
                    Thread.Sleep(delta);
	            }
	            lastTime = currentTime;

            }
            manager.Stop();
        }
    }
}

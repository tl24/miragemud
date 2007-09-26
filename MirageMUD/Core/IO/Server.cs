using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.Collections;
using System.Diagnostics;
using Mirage.Core.Data;
using Mirage.Core.Command;
using Mirage.Core.Communication;
using System.Threading;
using log4net;
using Mirage.Core.Util;
using Mirage.Core.IO.Serialization;

namespace Mirage.Core.IO
{
    public class Server
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
            manager.Configure();
            MudRepositoryBase globalLists = MudFactory.GetObject<MudRepositoryBase>();
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

                Queue<IPlayer> removePlayers = new Queue<IPlayer>();

                // reset state
                foreach (IPlayer player in globalLists.Players)
                {
                    player.Client.CommandRead = false;
                    player.Client.OutputWritten = false;
                    if (!player.Client.IsOpen)
                    {
                        SavePlayer(player);
                        removePlayers.Enqueue(player);
                    }
                }

                while (removePlayers.Count > 0)
                {
                    removePlayers.Dequeue().FirePlayerEvent(PlayerEventType.Quiting);
                    //globalLists.RemovePlayer(removePlayers.Dequeue());                    
                }

                foreach (IPlayer player in globalLists.Players)
                {
                    player.Client.ProcessInput();
                }

                foreach (IPlayer player in globalLists.Players)
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

        protected void SavePlayer(IPlayer player)
        {
            IPersistenceManager persister = ObjectStorageFactory.GetPersistenceManager(player.GetType());
            persister.Save(player, player.Uri);
        }
    }
}

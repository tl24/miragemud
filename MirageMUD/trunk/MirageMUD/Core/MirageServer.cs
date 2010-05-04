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
using System.Configuration;
using System.Collections.Specialized;
using Mirage.Core.IO;

namespace Mirage.Core
{
    public class MirageServer
    {
        private static ILog logger = LogManager.GetLogger(typeof(MirageServer));

        /// <summary>
        ///     The network port being listened on
        /// </summary>
        private int _port;

        /// <summary>
        ///     Stop flag to cause the Server loop to stop
        /// </summary>
        private bool _shutdown;

        private IInitializer[] _initializers;
        private List<ServiceEntry> _services;

        public MirageServer()
        {
            Shutdown = true;
        }

        public bool Shutdown {
            get { return _shutdown; }
            set { _shutdown = value; }
        }

        public IInitializer[] Initializers
        {
            get { return _initializers; }
            set { _initializers = value; }
        }

        protected void Init()
        {
            if (_initializers != null)
            {
               
                foreach (IInitializer initModule in _initializers)
                {
                    logger.Info("Loading initializer " + initModule.Name);
                    initModule.Execute();                    
                }                
            }
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
            Thread.CurrentThread.Name = "Main";
            logger.Info("Starting up");

            ClientManager manager = null;
            MudRepositoryBase globalLists = null;
            List<IMudClient> NannyClients = null;
            BlockingQueue<IMudClient> NannyQueue = null;
            DateTime lastTime;
            DateTime currentTime;
            TimeSpan delta;
            int loopCount = 0;

            //TODO: Read this from config
            int PulsePerSecond = 4;

            try
            {
                Init();
                manager = MudFactory.GetObject<ClientManager>();
                //manager.Configure();
                globalLists = MudFactory.GetObject<MudRepositoryBase>();
                NannyClients = new List<IMudClient>();
                // These are the new connections waiting to be put in the nanny list
                NannyQueue = new BlockingQueue<IMudClient>(15);

                manager.NewClients = NannyQueue;
                manager.Start();
            }
            catch (Exception e)
            {
                logger.Error("Exception occurred during mud startup", e);
                logger.Error("Shutting down");
                return;
            }

            lastTime = DateTime.Now;
            currentTime = DateTime.Now;
            delta = new TimeSpan();

            while (!_shutdown)
            {
                try
                {
                    loopCount++;
                    IMudClient newClient;

                    try
                    {
                        while (NannyQueue.TryDequeue(out newClient))
                        {
                            NannyClients.Add(newClient);
                        }
                    }
                    catch (Exception e)
                    {
                        logger.Error("Error trying to dequeue new client", e);
                    }

                    for (int i = NannyClients.Count - 1; i >= 0; i--)
                    {
                        try
                        {
                            if (NannyClients[i].IsOpen)
                            {
                                // Process input if still connected
                                NannyClients[i].ProcessInput();
                                if (NannyClients[i].Player != null && NannyClients[i].State == ConnectedState.Playing)
                                {
                                    // graduated...remove from the list
                                    NannyClients[i].WritePrompt();
                                    NannyClients.RemoveAt(i);
                                }
                            }
                            else
                            {
                                // Not connected, remove them
                                NannyClients.RemoveAt(i);
                            }
                        }
                        catch (Exception e)
                        {
                            logger.Error("Error processing nanny client", e);
                            NannyClients[i].Write(new StringMessage(MessageType.SystemError, "ProcessError", "Error occurred processing your request." + Environment.NewLine));
                            NannyClients[i].Close();
                        }
                    }

                    foreach (ServiceEntry service in Services)
                    {
                        if (!service.Service.IsStarted)
                            service.Service.Start();
                        service.Execute();
                    }

                }
                catch (Exception e)
                {
                    logger.Error("Unhandled exception in main loop", e);
                }
                currentTime = DateTime.Now;
	            delta = lastTime + TimeSpan.FromSeconds(1.0d/PulsePerSecond) - currentTime;
	            if (delta.Ticks > 0) {
	                //Thread.sleep($timedelta);
                    try
                    {
                        Thread.Sleep(delta);
                    }
                    catch (ThreadInterruptedException e)
                    {
                        break;
                    }
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

        public List<ServiceEntry> Services
        {
            get { return this._services; }
            set { this._services = value; }
        }
    }
}

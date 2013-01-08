using System;
using System.Collections.Generic;
using System.Threading;
using log4net;
using Mirage.Core.Collections;
using Mirage.Game.Communication;
using Mirage.Game.IO.Net;
using Mirage.Game.World;
using Mirage.Core.IO.Net;
using Mirage.Core.IO.Serialization;
using Mirage.Core.Messaging;

namespace Mirage.Game.Server
{
    public class MirageServer
    {
        private static ILog logger = LogManager.GetLogger(typeof(MirageServer));

        public MirageServer()
        {
            Shutdown = true;
        }

        /// <summary>
        ///     Stop flag to cause the Server loop to stop
        /// </summary>
        public bool Shutdown { get; set; }

        public IInitializer[] Initializers { get; set; }

        public IClientFactory AdapterFactory { get; set; }

        public ServiceProcessor Services { get; set; }

        protected void Init()
        {
            if (Initializers != null)
            {
               
                foreach (IInitializer initModule in Initializers)
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
            Shutdown = false;
            Thread.CurrentThread.Name = "Main";
            logger.Info("Starting up");

            ConnectionManager manager = null;
            MudWorld globalLists = null;
            List<IClient> NannyClients = null;
            BlockingQueue<IConnection> NannyQueue = null;
            DateTime lastTime;
            DateTime currentTime;
            TimeSpan delta;
            int loopCount = 0;

            //TODO: Read this from config
            int PulsePerSecond = 4;

            try
            {
                Init();
                //TODO: Create the server with the container
                manager = MudFactory.GetObject<ConnectionManager>();
                //manager.Configure();
                globalLists = MudFactory.GetObject<MudWorld>();
                AdapterFactory = MudFactory.GetObject<IClientFactory>();
                NannyClients = new List<IClient>();
                // These are the new connections waiting to be put in the nanny list
                NannyQueue = new BlockingQueue<IConnection>(15);

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

            while (!Shutdown)
            {
                try
                {
                    loopCount++;
                    
                    try
                    {
                        IConnection connection;
                        while (NannyQueue.TryDequeue(out connection))
                        {
                            NannyClients.Add(AdapterFactory.CreateConnectionAdapter(connection));
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
                                    //NannyClients[i].WritePrompt();
                                    //TODO: centralize this
                                    string clientName = NannyClients[i].Player.Name;
                                    NannyClients[i].Player.Client.Write(new StringMessage(MessageType.Prompt, "DefaultPrompt", clientName + ">> "));

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

                    if (!Services.IsStarted)
                        Services.Start();
                    Services.Process();

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
                    catch (ThreadInterruptedException)
                    {
                        break;
                    }
	            }
	            lastTime = currentTime;

            }

            if (Services.IsStarted)
            {
                Services.Stop();
            }
            manager.Stop();
            logger.Info("The mud has shutdown successfully.");
        }
    }
}

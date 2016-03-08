using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using Mirage.Core.IO.Net;
using System.Threading;
using Mirage.Core.Collections;
using System.Diagnostics;

namespace Mirage.Core.Server
{
    /// <summary>
    /// Base class for a mud server
    /// </summary>
    public abstract class ServerBase
    {
        protected static ILog logger = LogManager.GetLogger(typeof(ServerBase));

        public ServerBase(ConnectionManager connectionManager)
        {
            if (connectionManager == null)
                throw new ArgumentNullException("ConnectionManager");
            ConnectionManager = connectionManager;
            Shutdown = true;
            PulsePerSecond = 4;
        }

        public IInitializer[] Initializers { get; set; }

        public ConnectionManager ConnectionManager { get; private set; }

        /// <summary>
        ///     Stop flag to cause the Server loop to stop
        /// </summary>
        public bool Shutdown { get; set; }

        public int PulsePerSecond { get; set; }

        /// <summary>
        /// Calls any initializers that were registered
        /// </summary>
        protected virtual void Init()
        {
            if (Initializers != null)
            {
                foreach (IInitializer initModule in Initializers)
                {
                    logger.Info("Loading initializer " + initModule.GetType().Name);
                    initModule.Execute();
                }
                Initializers = null;
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

            BlockingQueue<IConnection> NannyQueue = null;
            try
            {
                Init();
                // These are the new connections waiting to be put in the nanny list
                NannyQueue = new BlockingQueue<IConnection>(15);

                ConnectionManager.NewClients = NannyQueue;
                ConnectionManager.Start();
            }
            catch (Exception e)
            {
                logger.Error("Exception occurred during mud startup", e);
                logger.Error("Shutting down");
                return;
            }

            Stopwatch stopwatch = new Stopwatch();
            while (!Shutdown)
            {
                stopwatch.Restart();
                try
                {
                    try
                    {
                        IConnection connection;
                        while (NannyQueue.TryDequeue(out connection))
                        {
                            OnNewConnection(connection);
                        }
                    }
                    catch (Exception e)
                    {
                        logger.Error("Error trying to dequeue new client", e);
                    }

                    ProcessLoop();
                }
                catch (Exception e)
                {
                    logger.Error("Unhandled exception in main loop", e);
                }
                stopwatch.Stop();
                TimeSpan delta = TimeSpan.FromSeconds(1.0d / PulsePerSecond) - stopwatch.Elapsed;
                if (delta.Ticks > 0)
                {
                    try
                    {
                        Thread.Sleep(delta);
                    }
                    catch (ThreadInterruptedException)
                    {
                        break;
                    }
                }
            }

            OnShutdown();
            ConnectionManager.Stop();
            logger.Info("The mud has shutdown successfully.");
        }

        protected abstract void OnNewConnection(IConnection connection);

        protected abstract void ProcessLoop();

        protected abstract void OnShutdown();
    }
}

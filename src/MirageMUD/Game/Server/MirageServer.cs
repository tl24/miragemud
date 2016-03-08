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
using Mirage.Core.Server;

namespace Mirage.Game.Server
{
    public class MirageServer : ServerBase
    {
        private List<IClient<ClientPlayerState>> NannyClients = new List<IClient<ClientPlayerState>>();

        public MirageServer(ConnectionManager connectionManager)
            : base(connectionManager)
        {
        }
        public IClientFactory ClientFactory { get; set; }

        public ServiceProcessor Services { get; set; }


        protected override void OnNewConnection(IConnection connection)
        {
            NannyClients.Add(ClientFactory.CreateConnectionAdapter(connection));
        }

        protected override void ProcessLoop()
        {
            for (int i = NannyClients.Count - 1; i >= 0; i--)
            {
                try
                {
                    if (NannyClients[i].IsOpen)
                    {
                        // Process input if still connected
                        NannyClients[i].ProcessInput();
                        if (NannyClients[i].ClientState.Player != null && NannyClients[i].ClientState.State == ConnectedState.Playing)
                        {
                            // graduated...remove from the list
                            //NannyClients[i].WritePrompt();
                            //TODO: centralize this
                            string clientName = NannyClients[i].ClientState.Player.Name;
                            NannyClients[i].ClientState.Player.Client.Write(new StringMessage(MessageType.Prompt, "DefaultPrompt", clientName + ">> "));

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

        protected override void OnShutdown()
        {
            if (Services.IsStarted)
            {
                Services.Stop();
            }
        }
    }
}

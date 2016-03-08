using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mirage.Core.Server;
using Mirage.Core.IO.Net;
using Mirage.Core.Messaging;

namespace SampleMud
{
    public class SampleMudServer : ServerBase
    {
        private List<TextClient> _newClients = new List<TextClient>();

        public SampleMudServer(ConnectionManager connectionManager)
            : base(connectionManager)
        {
        }

        protected override void OnNewConnection(Mirage.Core.IO.Net.IConnection connection)
        {
            var client = new TextClient((TextConnection)connection);
            _newClients.Add(client);
            client.Write(new StringMessage("Login", "Enter your name: "));
        }

        protected override void OnShutdown()
        {
            
        }

        protected override void ProcessLoop()
        {
            for (int i = _newClients.Count - 1; i >= 0; i--)
            {
                try
                {
                    if (_newClients[i].IsOpen)
                    {
                        // Process input if still connected
                        _newClients[i].ProcessInput();
                        if (_newClients[i].ClientState.Player != null)
                        {
                            // graduated...remove from the list
                            string clientName = _newClients[i].ClientState.Player.Name;
                            _newClients[i].ClientState.Player.Client.Write(new StringMessage(MessageType.Prompt, "DefaultPrompt", clientName + ">> "));

                            _newClients.RemoveAt(i);
                        }
                    }
                    else
                    {
                        // Not connected, remove them
                        _newClients.RemoveAt(i);
                    }
                }
                catch (Exception e)
                {
                    logger.Error("Error processing nanny client", e);
                    _newClients[i].Write(new StringMessage(MessageType.SystemError, "ProcessError", "Error occurred processing your request." + Environment.NewLine));
                    _newClients[i].Close();
                }
            }

            ClearPlayerFlagsAndRemove();
            ReadPlayerInput();
            WritePlayerOutput();
        }

        protected void ClearPlayerFlagsAndRemove()
        {
            Queue<Player> removePlayers = new Queue<Player>();

            // reset state
            foreach (Player player in World.Players)
            {
                player.Client.CommandRead = false;
                player.Client.OutputWritten = false;
                if (!player.Client.IsOpen)
                {
                    try
                    {
                        logger.InfoFormat("{0} has left the game.", player.Name);
                    }
                    catch (Exception e)
                    {
                        logger.Error("Error trying to save disconnected client before removing", e);
                    }
                    removePlayers.Enqueue(player);
                }
            }

            while (removePlayers.Count > 0)
            {
                try
                {
                    World.Players.Remove(removePlayers.Dequeue());
                }
                catch (Exception e)
                {
                    logger.Error("Error handling player quit event", e);
                }
            }
        }

        protected void ReadPlayerInput()
        {
            foreach (Player player in World.Players)
            {
                try
                {
                    player.Client.ProcessInput();
                }
                catch (Exception e)
                {
                    logger.Error("Error processing client input for player: " + player.Name, e);
                }
            }
        }

        protected void WritePlayerOutput()
        {
            foreach (Player player in World.Players)
            {
                try
                {
                    if (player.Client.CommandRead || player.Client.OutputWritten)
                    {
                        string clientName = player.Name;
                        player.Client.Write(new StringMessage(MessageType.Prompt, "DefaultPrompt", clientName + ">> "));
                    }
                }
                catch (Exception e)
                {
                    logger.Error("Error writing prompt for player: " + player.Name, e);
                }
            }
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mirage.Game.World;
using Castle.Core.Logging;
using Mirage.IO.Net;
using Mirage.Game.Communication;

namespace Mirage.Game.Server
{
    public class ServiceProcessor
    {
        private IPlayerRepository _playerRepository;

        private ILogger logger;
        public ILogger Logger
        {
            get { return logger ?? NullLogger.Instance; }
            set { logger = value; }
        }

        public IPlayerRepository PlayerRepository { get; set; }

        public MudWorld World { get; set; }

        public bool IsStarted { get; set; }

        public void Start()
        {
            IsStarted = true;
        }

        public void Stop()
        {
            SaveAllPlayers();
            IsStarted = false;
        }

        public void Process()
        {
            ClearPlayerFlagsAndRemove();
            ReadPlayerInput();
            ProcessMobileInput();
            WritePlayerOutput();
        }

        protected void ClearPlayerFlagsAndRemove()
        {
            Queue<IPlayer> removePlayers = new Queue<IPlayer>();

            // reset state
            foreach (IPlayer player in PlayerRepository)
            {
                player.Client.CommandRead = false;
                player.Client.OutputWritten = false;
                if (!player.Client.IsOpen)
                {
                    try
                    {
                        logger.InfoFormat("{0} has left the game.", player.Uri);
                        SavePlayer(player);
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
                    removePlayers.Dequeue().FirePlayerEvent(PlayerEventType.Quiting);
                }
                catch (Exception e)
                {
                    logger.Error("Error handling player quit event", e);
                }
            }
        }

        protected void ReadPlayerInput()
        {
            foreach (IPlayer player in PlayerRepository)
            {
                try
                {
                    player.Client.ProcessInput();
                }
                catch (Exception e)
                {
                    logger.Error("Error processing client input for player: " + player.Uri, e);
                }
            }
        }

        protected void WritePlayerOutput()
        {
            foreach (IPlayer player in PlayerRepository)
            {
                try
                {
                    if (player.Client.CommandRead || player.Client.OutputWritten)
                    {
                        if (player.Client.State == ConnectedState.Playing)
                        {
                            string clientName = player.Uri;
                            player.Client.Write(new StringMessage(MessageType.Prompt, "DefaultPrompt", clientName + ">> "));
                        }
                    }
                }
                catch (Exception e)
                {
                    logger.Error("Error writing prompt for player: " + player.Uri, e);
                }
            }
        }


        protected void ProcessMobileInput()
        {
            foreach (Mobile mob in World.Mobiles)
            {
                mob.ProcessInput();
            }
        }

        protected void SavePlayer(IPlayer player)
        {
            PlayerRepository.Save(player);
        }

        protected void SaveAllPlayers()
        {
            foreach (IPlayer player in PlayerRepository)
            {
                try
                {
                    logger.InfoFormat("Saving player {0}.", player.Uri);
                    SavePlayer(player);
                }
                catch (Exception e)
                {
                    logger.Error("Error trying to save player before stopping", e);
                }
            }
        }

    }
}

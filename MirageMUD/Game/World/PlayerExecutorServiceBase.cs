using System;
using System.Collections.Generic;
using Castle.Core.Logging;
using Mirage.Game.Communication;
using Mirage.IO.Net;

namespace Mirage.Game.World
{
    public class PlayerExecutorServiceBase : ServiceExecutorBase
    {
        //private ILog logger = LogManager.GetLogger(typeof(PlayerExecutorServiceBase));
        private IPlayerRepository _playerRepository;

        private ILogger logger;
        public ILogger Logger
        {
            get { return logger ?? NullLogger.Instance; }
            set { logger = value; }
        }

        public IPlayerRepository PlayerRepository
        {
            get { return this._playerRepository; }
            set { this._playerRepository = value; }
        }

        public override ServiceMethod GetServiceMethod(string key)
        {
            switch (key.ToLower())
            {
                case "clearflagsandremove":
                    return ClearFlagsAndRemove;
                case "readinput":
                    return ReadInput;
                case "writeoutput":
                    return WriteOutput;
                default:
                    throw new ArgumentException("There is no service method associated with the key: " + key);
            }
        }

        public void ClearFlagsAndRemove()
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

        public void ReadInput()
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

        public void WriteOutput()
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

        protected void SavePlayer(IPlayer player)
        {
            PlayerRepository.Save(player);
        }

        protected override void OnStop()
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

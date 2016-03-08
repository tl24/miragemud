using System.Configuration;
using log4net;
using Mirage.Core.Messaging;
using Mirage.Game.Communication;
using Mirage.Game.World;
using Mirage.Game.World.Query;
using Mirage.Core.IO.Net;

namespace Mirage.Game.IO.Net
{
    /// <summary>
    /// Finalizes player state after login or new creation
    /// </summary>
    public class PlayerFinalizer
    {
        private static ILog logger = LogManager.GetLogger(typeof(PlayerFinalizer));

        /// <summary>
        /// Finalizes the player
        /// </summary>
        /// <param name="isNew">true if this is a new player, false for existing</param>
        /// <returns>true if successful, false if validation failed, which should trigger a disconnect</returns>
        public static bool Finalize(bool isNew, Player player, IClient<ClientPlayerState> client)
        {
            if (CheckAlreadyPlaying(player, client))
            {
                return false;
            }
            else
            {
                //TODO: probably need to pick a different logger here
                LogManager.GetLogger(typeof(ClientBase<ClientPlayerState>)).Info(string.Format("{0}@{1} has connected.", player.Uri, client.Address));
                IChannelRepository channelRepository = MudFactory.GetObject<IChannelRepository>();
                IPlayerRepository playerRepository = MudFactory.GetObject<IPlayerRepository>();
                if (isNew)
                {
                    player.Roles = new string[] { "player" };
                    // default channels
                    foreach (Channel channel in channelRepository)
                    {
                        if (channel.IsDefault)
                        {
                            // don't worry about security here, we'll try and actually join them down below
                            // which will tell us whether we can really have the channel on
                            player.CommunicationPreferences.ChannelOn(channel.Name);
                        }
                    }
                }

                
                playerRepository.Add(player);
                if (player.Room == null)
                {
                    Room defaultRoom = (Room)MudFactory.GetObject<MudWorld>().ResolveUri(ConfigurationManager.AppSettings["default.room"]);
                    defaultRoom.Add(player);
                }
                else
                {
                    player.Room.Add(player);
                }

                client.Write(player.ForSelf(CommonMessages.Welcome));
                // Try to turn on channels
                foreach (Channel channel in channelRepository)
                {
                    if (player.CommunicationPreferences.IsChannelOn(channel.Name))
                    {
                        // check to see that the player can still join the channel
                        if (channel.CanJoin(player))
                        {
                            channel.Add(player);
                        }
                        else
                        {
                            // can't join, so turn it off in their preferences
                            player.CommunicationPreferences.ChannelOff(channel.Name);
                        }
                    }
                }
                client.ClientState.State = ConnectedState.Playing;
                client.ClientState.Player = player;
                player.Client = client;
                logger.Info("Player " + player.Uri + " has joined the game.");
                return true;
            }
        }

        private static bool CheckAlreadyPlaying(Player player, IClient<ClientPlayerState> client)
        {
            Player isPlaying = (Player)MudFactory.GetObject<MudWorld>().Players.FindOne(player.Uri, QueryMatchType.Exact);
            if (isPlaying != null && isPlaying.Client.ClientState.State == ConnectedState.Playing)
            {
                client.Write(MessageFormatter.Instance.Format(null, null, LoginAndPlayerCreationMessages.PlayerAlreadyPlaying));
                client.ClientState.Player = null;
                client.ClientState.State = ConnectedState.Connecting;
                client.Close();
                return true;
            }
            return false;
        }
    }
}

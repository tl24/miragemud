using System.Configuration;
using log4net;
using Mirage.Game.Communication;
using Mirage.Game.World;
using Mirage.Game.World.Query;
using Mirage.IO.Net;

namespace Mirage.Game.IO.Net
{
    /// <summary>
    /// Finalizes player state after login or new creation
    /// </summary>
    public class PlayerFinalizer
    {
        private static ILog logger = LogManager.GetLogger(typeof(PlayerFinalizer));
        private Player _player;
        private IConnectionAdapter _client;

        public PlayerFinalizer(IConnectionAdapter client, Player player)
        {
            _player = player;
            _client = client;
        }

        public Player Player
        {
            get { return _player; }
        }

        public IConnectionAdapter Client
        {
            get { return _client; }
        }

        /// <summary>
        /// Finalizes the player
        /// </summary>
        /// <param name="isNew">true if this is a new player, false for existing</param>
        /// <returns>true if successful, false if validation failed, which should trigger a disconnect</returns>
        public bool Finalize(bool isNew)
        {
            if (CheckAlreadyPlaying())
            {
                return false;
            }
            else
            {
                //TODO: probably need to pick a different logger here
                LogManager.GetLogger(typeof(ConnectionAdapterBase)).Info(string.Format("{0}@{1} has connected.", Player.Uri, Client.Address));
                IChannelRepository channelRepository = MudFactory.GetObject<IChannelRepository>();
                IPlayerRepository playerRepository = MudFactory.GetObject<IPlayerRepository>();
                if (isNew)
                {
                    Player.Roles = new string[] { "player" };
                    // default channels
                    foreach (Channel channel in channelRepository)
                    {
                        if (channel.IsDefault)
                        {
                            // don't worry about security here, we'll try and actually join them down below
                            // which will tell us whether we can really have the channel on
                            Player.CommunicationPreferences.ChannelOn(channel.Name);
                        }
                    }
                }

                
                playerRepository.Add(Player);
                if (Player.Container == null)
                {
                    Room defaultRoom = (Room)MudFactory.GetObject<IQueryManager>().Find(ConfigurationManager.AppSettings["default.room"]);
                    defaultRoom.Add(Player);
                }
                else
                {
                    Player.Container.Add(Player);
                }

                Client.Write(MudFactory.GetObject<IMessageFactory>().GetMessage("negotiation.welcome"));
                // Try to turn on channels
                foreach (Channel channel in channelRepository)
                {
                    if (Player.CommunicationPreferences.IsChannelOn(channel.Name))
                    {
                        // check to see that the player can still join the channel
                        if (channel.CanJoin(Player))
                        {
                            channel.Add(Player);
                        }
                        else
                        {
                            // can't join, so turn it off in their preferences
                            Player.CommunicationPreferences.ChannelOff(channel.Name);
                        }
                    }
                }
                Client.State = ConnectedState.Playing;
                Client.Player = Player;
                Player.Client = Client;
                logger.Info("Player " + Player.Uri + " has joined the game.");
                return true;
            }
        }

        public bool CheckAlreadyPlaying()
        {
            Player isPlaying = (Player)MudFactory.GetObject<IQueryManager>().Find(new ObjectQuery(null, "Players", new ObjectQuery(Player.Uri)));
            if (isPlaying != null && isPlaying.Client.State == ConnectedState.Playing)
            {
                Client.Write(MudFactory.GetObject<IMessageFactory>().GetMessage("negotiation.authentication.PlayerAlreadyPlaying"));
                Client.Player = null;
                Client.State = ConnectedState.Connecting;
                Client.Close();
                return true;
            }
            return false;
        }
    }
}

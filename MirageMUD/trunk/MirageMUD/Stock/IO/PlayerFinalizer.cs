using System;
using System.Collections.Generic;
using System.Text;
using Mirage.Stock.Data;
using Mirage.Core.Communication;
using Mirage.Core.IO;
using Mirage.Core.Data;
using Mirage.Core.Data.Query;
using System.Configuration;

namespace Mirage.Stock.IO
{
    /// <summary>
    /// Finalizes player state after login or new creation
    /// </summary>
    public class PlayerFinalizer
    {
        private Player _player;
        private IClient _client;

        public PlayerFinalizer(IClient client, Player player)
        {
            _player = player;
            _client = client;
        }

        public Player Player
        {
            get { return _player; }
        }

        public IClient Client
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
                Client.Logger.Info(string.Format("{0}@{1} has connected.", Player.Uri, Client.TcpClient.Client.LocalEndPoint));
                MudRepositoryBase globalLists = MudFactory.GetObject<MudRepositoryBase>();
                if (isNew)
                {
                    Player.Roles = new string[] { "player" };
                    // default channels
                    foreach (Channel channel in globalLists.Channels)
                    {
                        if (channel.IsDefault)
                        {
                            // don't worry about security here, we'll try and actually join them down below
                            // which will tell us whether we can really have the channel on
                            Player.CommunicationPreferences.ChannelOn(channel.Name);
                        }
                    }
                }

                
                globalLists.AddPlayer(Player);
                if (Player.Container == null)
                {
                    Room defaultRoom = (Room)MudFactory.GetObject<QueryManager>().Find(ConfigurationManager.AppSettings["default.room"]);
                    defaultRoom.Add(Player);
                }
                else
                {
                    Player.Container.Add(Player);
                }

                Client.Write(MessageFactory.GetMessage("msg:/negotiation/welcome"));
                // Try to turn on channels
                foreach(Channel channel in globalLists.Channels) {
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
                return true;
            }
        }

        public bool CheckAlreadyPlaying()
        {
            Player isPlaying = (Player)MudFactory.GetObject<QueryManager>().Find(new ObjectQuery(null, "Players", new ObjectQuery(Player.Uri)));
            if (isPlaying != null && isPlaying.Client.State == ConnectedState.Playing)
            {
                Client.Write(MessageFactory.GetMessage("msg:/negotiation/authentication/player.already.playing"));
                Client.Player = null;
                Client.State = ConnectedState.Connecting;
                Client.Close();
                return true;
            }
            return false;
        }
    }
}

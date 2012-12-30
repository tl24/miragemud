using System.Text;
using Mirage.Game.Communication;
using Mirage.Game.World;
using Mirage.Game.World.Query;

namespace Mirage.Game.Command
{
    /// <summary>
    /// Core mud commands
    /// </summary>
    public class MudCommands : CommandDefaults
    {
        public IMessageFactory MessageFactory { get; set; }

        public IChannelRepository ChannelRespository  { get; set; }

        /// <summary>
        /// Lists the available channels
        /// </summary>
        /// <returns></returns>
        [Command]
        public IMessage channels([Actor] IActor actor)
        {
            StringBuilder sb = new StringBuilder();
            string format = "{0,-20}  {1,-10}\r\n";
            sb.AppendFormat(format, "Channel Name", "Status");
            sb.AppendFormat("--------------------  ----------\r\n");
            foreach (Channel channel in ChannelRespository)
            {
                if (channel.CanJoin(actor))
                {
                    if (channel.ContainsMember(actor))
                        sb.AppendFormat(format, channel.Name, "on");
                    else
                        sb.AppendFormat(format, channel.Name, "off");
                }
            }
            return MessageFactory.GetMessage("communication.ChannelList", sb.ToString());
        }

        /// <summary>
        /// Shows ignore list
        /// </summary>
        /// <param name="actor">player</param>
        /// <returns></returns>
        [Command(Description="Shows list of ignored players")]
        public IMessage ignore([Actor] IPlayer actor)
        {
            StringBuilder sb = new StringBuilder();
            if (actor.CommunicationPreferences.Ignored.Count > 0)
            {
                sb.AppendLine("You are currently ignoring:");
                foreach (string name in actor.CommunicationPreferences.Ignored)
                {
                    sb.AppendLine(name);
                }
            }
            else
            {
                sb.AppendLine("You're not currently ignoring anyone.");
            }
            return MessageFactory.GetMessage("communication.IgnoreList", sb.ToString());
        }

        [Command(Description = "Toggles ignoring a player on or off")]
        public IMessage ignore([Actor] IPlayer actor, string player)
        {
            IMessage message;
            if (actor.CommunicationPreferences.IsIgnored(player))
            {
                // they're ignored, so stop ignoring them
                message = MessageFactory.GetMessage("communication.UnignorePlayer");                
                actor.CommunicationPreferences.UnIgnore(player);
            }
            else
            {
                // they're not ignored, so start ignoring them
                // try and find them first to validate its a valid name
                IPlayer p = (IPlayer) World.Players.FindOne(player);
                if (p == null)
                {
                    // they're not playing
                    message = MessageFactory.GetMessage("common.error.PlayerNotPlaying");
                }
                else
                {
                    if (p == actor)
                    {
                        return MessageFactory.GetMessage("communication.CantIgnoreSelf");
                    }
                    // found them, ignore them
                    message = MessageFactory.GetMessage("communication.IgnorePlayer");
                    actor.CommunicationPreferences.Ignore(p.Uri);
                    player = p.Uri;
                }
            }
            message["player"] = player;
            return message;
        }
    }
}

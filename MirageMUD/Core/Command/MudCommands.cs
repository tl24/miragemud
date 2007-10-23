using System;
using System.Collections.Generic;
using System.Text;
using Mirage.Core.Communication;
using Mirage.Core.Data;
using Mirage.Core.Data.Query;

namespace Mirage.Core.Command
{
    /// <summary>
    /// Core mud commands
    /// </summary>
    public class MudCommands
    {
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
            MudRepositoryBase repository = MudFactory.GetObject<MudRepositoryBase>();
            foreach (Channel channel in repository.Channels)
            {
                if (channel.CanJoin(actor))
                {
                    if (channel.ContainsMember(actor))
                        sb.AppendFormat(format, channel.Name, "on");
                    else
                        sb.AppendFormat(format, channel.Name, "off");
                }
            }
            return new StringMessage(MessageType.Communication, Namespaces.Communication, "channel.list", sb.ToString());
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
            return new StringMessage(MessageType.Communication, Namespaces.Communication, "ignore.list", sb.ToString());
        }

        [Command(Description = "Toggles ignoring a player on or off")]
        public IMessage ignore([Actor] IPlayer actor, string player)
        {
            ResourceMessage rm;
            if (actor.CommunicationPreferences.IsIgnored(player))
            {
                // they're ignored, so stop ignoring them
                rm = (ResourceMessage) MessageFactory.GetMessage("msg:/communication/unignore.player");                
                actor.CommunicationPreferences.UnIgnore(player);
            }
            else
            {
                // they're not ignored, so start ignoring them
                // try and find them first to validate its a valid name
                IPlayer p = (IPlayer) MudFactory.GetObject<QueryManager>().Find(ObjectQuery.parse("/Players", player));
                if (p == null)
                {
                    // they're not playing
                    rm = (ResourceMessage)MessageFactory.GetMessage("msg:/common/error/PlayerNotPlaying");
                }
                else
                {
                    if (p == actor)
                    {
                        return MessageFactory.GetMessage("msg:/communication/cant.ignore.self");
                    }
                    // found them, ignore them
                    rm = (ResourceMessage)MessageFactory.GetMessage("msg:/communication/ignore.player");
                    actor.CommunicationPreferences.Ignore(p.Uri);
                    player = p.Uri;
                }
            }
            rm["player"] = player;
            return rm;
        }
    }
}

using System.Text;
using Mirage.Game.Communication;
using Mirage.Game.World;
using Mirage.Game.World.Query;

namespace Mirage.Game.Command
{
    /// <summary>
    /// Core mud commands
    /// </summary>
    public class CommunicationCommands : CommandDefaults
    {
        public class Messages
        {
            public static readonly MessageDefinition SayOthers = new MessageDefinition("communication.say.others", "${actor} says '${message}'.");
            public static readonly MessageDefinition SaySelf = new MessageDefinition("communication.say.self", "You said '${message}'.");
            public static readonly MessageDefinition TellOthers = new MessageDefinition("communication.tell.others", "${actor} tells you '${message}'.");
            public static readonly MessageDefinition TellSelf = new MessageDefinition("communication.tell.self", "You tell ${target} '${message}'.");

            public static readonly MessageDefinition BeingIgnored = new MessageDefinition("communication.common.beingignored", "${target} is ignoring you.");

            public static readonly MessageDefinition IgnoreTarget = new MessageDefinition("communication.ignore.self.ignored", "You are now ignoring all communication from ${target}.");
            public static readonly MessageDefinition UnignoreTarget = new MessageDefinition("communication.ignore.self.unignored", "You are no longer ignoring ${target}.");
            public static readonly MessageDefinition IgnoreErrorSelf = new MessageDefinition("communication.ignore.self.error.cantignoreself", "You can't ignore yourself!");

            // Id's for custom formatted messages
            public static readonly string ChannelList = "communication.channels";
            public static readonly string IgnoreList = "communication.ignorelist";
        }

        /// <summary>
        ///     Say something to everyone in the room
        /// </summary>
        /// <param name="actor">the player speaking</param>
        /// <param name="args">the message to speak</param>
        /// <param name="extraArgs"></param>
        [Command(Aliases = new string[] { "'", "say" })]
        public void say([Actor] Living actor, [CustomParse] string message)
        {
            //speak to all others in the room
            //IMessage msgToOthersTemplate = MessageFactory.GetMessage("communication.SayOthers");
            //msgToOthersTemplate["player"] = actor.Title;
            //msgToOthersTemplate["message"] = message;
            foreach (Living am in actor.Room.LivingThings)
            {
                if (am != actor)
                {
                    if (am is Player && ((Player)am).CommunicationPreferences.IsIgnored(actor.Uri))
                        continue;
                    actor.ToTarget(Messages.SayOthers, am, new { message });
                    //IMessage m = msgToOthersTemplate.Copy();
                    //m["player"] = ViewManager.GetTitle(am, actor);
                    //am.Write(m);
                }
            }

            //repeat message to yourself as confirmation
            //IMessage msgToSelf = MessageFactory.GetMessage("communication.SaySelf");
            //msgToSelf["message"] = message;
            //return msgToSelf;
            actor.ToSelf(Messages.SaySelf, null, new { message });
        }

        [Command]
        public void tell([Actor] Living actor, string target, [CustomParse] string message)
        {
            // look up the target
            Player p = (Player)World.Players.FindOne(target, QueryMatchType.Exact);

            if (p == null)
            {
                // couldn't find them, send an error
                actor.ToSelf(CommonMessages.ErrorPlayerNotPlaying, target);
            }
            else
            {
                if (p.CommunicationPreferences.IsIgnored(actor.Uri)
                    && !actor.Principal.IsInRole("immortal"))
                {
                    //They're ignoring us!
                    actor.ToSelf(Messages.BeingIgnored, p);
                }
                else
                {
                    // format the messages
                    actor.ToTarget(Messages.TellOthers, p, new { message });
                    actor.ToSelf(Messages.TellSelf, p, new { message });
                }
            }
        }

        /// <summary>
        /// Lists the available channels
        /// </summary>
        /// <returns></returns>
        [Command]
        public void channels([Actor] Living actor)
        {
            StringBuilder sb = new StringBuilder();
            string format = "{0,-20}  {1,-10}\r\n";
            sb.AppendFormat(format, "Channel Name", "Status");
            sb.AppendFormat("--------------------  ----------\r\n");
            foreach (Channel channel in World.Channels)
            {
                if (channel.CanJoin(actor))
                {
                    if (channel.ContainsMember(actor))
                        sb.AppendFormat(format, channel.Name, "on");
                    else
                        sb.AppendFormat(format, channel.Name, "off");
                }
            }
            actor.ToSelf(Messages.ChannelList, sb.ToString());
        }

        /// <summary>
        /// Shows ignore list
        /// </summary>
        /// <param name="actor">player</param>
        /// <returns></returns>
        [Command(Description="Shows list of ignored players")]
        public void ignore([Actor] Player actor)
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
            actor.ToSelf(Messages.IgnoreList, sb.ToString());
        }

        [Command(Description = "Toggles ignoring a player on or off")]
        public void ignore([Actor] Player actor, string target)
        {
            IMessage message;
            if (actor.CommunicationPreferences.IsIgnored(target))
            {
                // they're ignored, so stop ignoring them
                actor.ToSelf(Messages.UnignoreTarget, target);
                actor.CommunicationPreferences.UnIgnore(target);
                return;
            }
            else
            {
                // they're not ignored, so start ignoring them
                // try and find them first to validate its a valid name
                IPlayer p = (IPlayer) World.Players.FindOne(target);
                if (p == null)
                {
                    // they're not playing
                    actor.ToSelf(CommonMessages.ErrorPlayerNotPlaying, target);
                    return;
                }
                else
                {
                    if (p == actor)
                    {
                        actor.ToSelf(Messages.IgnoreErrorSelf);
                        return;
                    }
                    // found them, ignore them
                    actor.ToSelf(Messages.IgnoreTarget, p.Uri);
                    actor.CommunicationPreferences.Ignore(p.Uri);
                    return;
                }
            }
        }
    }
}

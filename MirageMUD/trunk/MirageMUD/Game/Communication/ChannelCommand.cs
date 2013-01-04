using System.Linq;
using Mirage.Game.Command;
using Mirage.Game.World;

namespace Mirage.Game.Communication
{
    /// <summary>
    /// Base class for channel commands
    /// </summary>
    public abstract class ChannelCommandBase : CommandBase
    {           
        public ChannelCommandBase(Channel channel)
        {
            Channel = channel;
            Name = channel.Name;
            Aliases = new string[0];
            var tmpRoles = new System.Collections.Generic.HashSet<string>(channel.Roles);
            Roles = tmpRoles.ToArray();
        }

        /// <summary>
        /// Turns the channel on for the actor if its not on
        /// </summary>
        /// <param name="sendConfirmation">True to send a confirmation to the actor that the channel was turned on</param>
        protected void ChannelOn(IActor actor, bool sendConfirmation)
        {
            if (!sendConfirmation || !Channel.ContainsMember(actor))
            {
                Channel.Add(actor);
                if (actor is IPlayer)
                {
                    ((IPlayer)actor).CommunicationPreferences.ChannelOn(Channel.Name);
                }
                if (sendConfirmation)
                {
                    actor.ToSelf(Channel.Messages.ChannelOn, null, new { channel = Channel.Name });
                }
            }
        }

        /// <summary>
        /// Turns the channel off for the actor if its not off
        /// </summary>
        /// <param name="sendConfirmation">True to send a confirmation to the actor that the channel was turned off</param>
        protected void ChannelOff(IActor actor, bool sendConfirmation)
        {
            if (!sendConfirmation || Channel.ContainsMember(actor))
            {
                Channel.Remove(actor);
                if (actor is IPlayer)
                {
                    ((IPlayer)actor).CommunicationPreferences.ChannelOff(Channel.Name);
                }
                if (sendConfirmation)
                {
                    actor.ToSelf(Channel.Messages.ChannelOff, null, new { channel = Channel.Name });
                }
            }
        }

        /// <summary>
        /// The channel for this command
        /// </summary>
        public Channel Channel { get; private set; }
    }

    /// <summary>
    /// Command to send a message to a channel, if the channel is not on
    /// the actor will attempt to join the channel
    /// </summary>
    public class ChannelSendCommand : ChannelCommandBase
    {
        public ChannelSendCommand(Channel channel)
            : base(channel)
        {
            ArgCount = 1;
            CustomParse = true;            
        }

        public override IMessage Invoke(string invokedName, IActor actor, object[] arguments)
        {
            if (!Channel.ContainsMember(actor))
            {
                ChannelOn(actor, false);
            }
            Channel.Send(actor, (string) arguments[0]);
            return null;    // confirmation?
        }

        public override string UsageHelp()
        {
            return "Usage: " + Name + " message\r\n";
        }

        public override string ShortHelp()
        {
            return Name + " Send a message to this channel";
        }
    }

    /// <summary>
    /// Toggles a channel on or off
    /// </summary>
    public class ChannelToggleCommand : ChannelCommandBase
    {
        public ChannelToggleCommand(Channel channel)
            : base(channel)
        {
            ArgCount = 0;
        }

        public override IMessage Invoke(string invokedName, IActor actor, object[] arguments)
        {
            if (!Channel.ContainsMember(actor))
            {
                ChannelOn(actor, true);
                return null;
            }
            else
            {
                ChannelOff(actor, true);
                return null;
            }
        }
        public override string UsageHelp()
        {
            return "Usage: " + Name + "\r\n";
        }

        public override string ShortHelp()
        {
            return Name + " Toggles the channel on or off";
        }
    }
}

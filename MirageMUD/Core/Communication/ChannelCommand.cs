using System;
using System.Collections.Generic;
using System.Text;
using Mirage.Core.Command;
using Mirage.Core.Util;
using Mirage.Core.Data;

namespace Mirage.Core.Communication
{
    /// <summary>
    /// Base class for channel commands
    /// </summary>
    public abstract class ChannelCommandBase : CommandBase
    {
        private Channel _channel;

        public ChannelCommandBase(Channel channel)
        {
            _channel = channel;
            _name = channel.Name;
            _aliases = new string[0];
            HashSet<string> tmpRoles = new HashSet<string>(channel.Roles);
            _roles = tmpRoles.ToArray();
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
                    ResourceMessage rm = (ResourceMessage)MessageFactory.GetMessage("msg:/communication/channel.on");
                    rm["channel"] = Channel.Name;
                    actor.Write(rm);
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
                    ResourceMessage rm = (ResourceMessage)MessageFactory.GetMessage("msg:/communication/channel.off");
                    rm["channel"] = Channel.Name;
                    actor.Write(rm);
                }
            }
        }

        /// <summary>
        /// The channel for this command
        /// </summary>
        public Channel Channel
        {
            get { return _channel; }
        }
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
            _argCount = 1;
            _customParse = true;            
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
            _argCount = 0;
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

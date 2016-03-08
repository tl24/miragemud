using System;
using System.Collections.Generic;
using Mirage.Game.Communication;
using Mirage.Core;
using Mirage.Game.World.Containers;
using System.Linq;
using Mirage.Core.Messaging;

namespace Mirage.Game.World
{
    public static class MessageSendingExtensions
    {

        /// <summary>
        /// Formats a message to the character only
        /// </summary>
        /// <param name="actor">the actor and recipient</param>
        /// <param name="messageDefinition">The message definition</param>
        /// <param name="target">The target parameter</param>
        /// <param name="args">an optional list of arguments used in the format string</param>
        public static IMessage ForSelf(this IReceiveMessages actor, MessageDefinition messageDefinition, object target = null, object anonymousTypeAsArgs = null)
        {
            IMessage msg = MessageFormatter.Instance.Format(actor, actor, messageDefinition, target, FormatArgs(anonymousTypeAsArgs));
            return msg;
        }
        /// <summary>
        /// Sends a message to the character only
        /// </summary>
        /// <param name="actor">the actor and recipient</param>
        /// <param name="messageDefinition">The message definition</param>
        /// <param name="target">The target parameter</param>
        /// <param name="args">an optional list of arguments used in the format string</param>
        public static void ToSelf(this IReceiveMessages actor, MessageDefinition messageDefinition, object target = null, object anonymousTypeAsArgs = null)
        {
            IMessage msg = MessageFormatter.Instance.Format(actor, actor, messageDefinition, target, FormatArgs(anonymousTypeAsArgs));
            if (msg != null)
                actor.Write(msg);
        }

        /// <summary>
        /// Formats a message for the character only
        /// </summary>
        /// <param name="actor">the actor and recipient</param>
        /// <param name="messageID">The message ID of the message, ".self" will be appended to it</param>
        /// <param name="formatString">Format of the message string or template</param>
        /// <param name="target">The target parameter</param>
        /// <param name="args">an optional list of arguments used in the format string</param>
        public static IMessage ForSelf(this IReceiveMessages actor, string messageID, string formatString, object target = null, object anonymousTypeAsArgs = null)
        {
            IMessage msg = MessageFormatter.Instance.Format(actor, actor, messageID + ".self", formatString, target, FormatArgs(anonymousTypeAsArgs));
            return msg;
        }

        /// <summary>
        /// Sends a message to the character only
        /// </summary>
        /// <param name="actor">the actor and recipient</param>
        /// <param name="messageID">The message ID of the message, ".self" will be appended to it</param>
        /// <param name="formatString">Format of the message string or template</param>
        /// <param name="target">The target parameter</param>
        /// <param name="args">an optional list of arguments used in the format string</param>
        public static void ToSelf(this IReceiveMessages actor, string messageID, string formatString, object target = null, object anonymousTypeAsArgs = null)
        {
            IMessage msg = MessageFormatter.Instance.Format(actor, actor, messageID + ".self", formatString, target, FormatArgs(anonymousTypeAsArgs));
            if (msg != null)
                actor.Write(msg);
        }

        /// <summary>
        /// Sends a message to the target only
        /// </summary>
        /// <param name="actor">the actor</param>
        /// <param name="messageDefinition">The message definition</param>
        /// <param name="target">The target and recipient</param>
        /// <param name="args">an optional list of arguments used in the format string</param>
        public static void ToTarget(this IReceiveMessages actor, MessageDefinition messageDefinition, IReceiveMessages target = null, object anonymousTypeAsArgs = null)
        {
            IMessage msg = MessageFormatter.Instance.Format(target, actor, messageDefinition, target, FormatArgs(anonymousTypeAsArgs));
            if (msg != null)
                target.Write(msg);
        }

        /// <summary>
        /// Sends a message to the target only
        /// </summary>
        /// <param name="actor">the actor</param>
        /// <param name="messageID">The message ID of the message, ".self" will be appended to it</param>
        /// <param name="formatString">Format of the message string or template</param>
        /// <param name="target">The target and recipient</param>
        /// <param name="args">an optional list of arguments used in the format string</param>
        public static void ToTarget(this IReceiveMessages actor, string messageID, string formatString, IReceiveMessages target = null, object anonymousTypeAsArgs = null)
        {
            IMessage msg = MessageFormatter.Instance.Format(target, actor, messageID + ".target", formatString, target, FormatArgs(anonymousTypeAsArgs));
            if (msg != null)
                target.Write(msg);
        }

        /// <summary>
        /// Formats a param array into the dictionary the formatter expects
        /// </summary>
        /// <param name="args">param list of args</param>
        /// <returns>Dictionary</returns>
        private static IDictionary<string, object> FormatArgs(object[] args)
        {
            if (args == null || args.Length == 0)
                return null;

            var dct = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            for (int i = 0; i < args.Length; i++)
            {
                dct[i == 0 ? "object" : "object" + i] = args[i];
            }
            return dct;
        }

        /// <summary>
        /// Formats a param array into the dictionary the formatter expects
        /// </summary>
        /// <param name="args">param list of args</param>
        /// <returns>Dictionary</returns>
        private static IDictionary<string, object> FormatArgs(object argsAsAnonymousType)
        {
            return ReflectionUtils.ObjectToDictionary(argsAsAnonymousType);
        }

        public static void ToBystanders(this IReceiveMessages actor, MessageDefinition messageDefinition, Living target = null, object anonymousTypeAsArgs = null)
        {
            ToBystanders(actor, null, messageDefinition, target, anonymousTypeAsArgs);
        }

        public static void ToBystanders(this IReceiveMessages actor, IContainer room, MessageDefinition messageDefinition, Living target = null, object anonymousTypeAsArgs = null)
        {
            ToRoomImpl(actor, room, messageDefinition.Name, messageDefinition.Text, target, false, anonymousTypeAsArgs);
        }

        public static void ToBystanders(this IReceiveMessages actor, string messageID, string formatString, Living target = null, object anonymousTypeAsArgs = null)
        {
            ToRoomImpl(actor, null, messageID, formatString, target, false, anonymousTypeAsArgs);
        }

        public static void ToBystanders(this IReceiveMessages actor, IContainer room, string messageID, string formatString, Living target = null, object anonymousTypeAsArgs = null)
        {
            ToRoomImpl(actor, room, messageID, formatString, target, false, anonymousTypeAsArgs);
        }

        public static void ToRoom(this IReceiveMessages actor, MessageDefinition messageDefinition, Living target = null, object anonymousTypeAsArgs = null)
        {
            ToRoom(actor, null, messageDefinition, target, anonymousTypeAsArgs);
        }

        public static void ToRoom(this IReceiveMessages actor, string messageID, string formatString, Living target = null, object anonymousTypeAsArgs = null)
        {
            ToRoomImpl(actor, null, messageID, formatString, target, true, anonymousTypeAsArgs);
        }

        public static void ToRoom(this IReceiveMessages actor, IContainer room, MessageDefinition messageDefinition, Living target = null, object anonymousTypeAsArgs = null)
        {
            ToRoomImpl(actor, room, messageDefinition.Name, messageDefinition.Text, target, true, anonymousTypeAsArgs);
        }

        public static void ToRoom(this IReceiveMessages actor, IContainer room, string messageID, string formatString, Living target = null, object anonymousTypeAsArgs = null)
        {
            ToRoomImpl(actor, room, messageID, formatString, target, true, anonymousTypeAsArgs);
        }

        private static void ToRoomImpl(this IReceiveMessages actor, IContainer room, string messageID, string formatString, Living target, bool includeTarget, object anonymousTypeAsArgs = null)
        {
            var dct = FormatArgs(anonymousTypeAsArgs);
            if (actor == null)
                return;
            if (room == null && (!(actor is IContainable) || ((IContainable)actor).Container == null))
                return;

            IContainer container = room ?? ((IContainable)actor).Container;

            foreach (IReceiveMessages liv in container.OfType<IReceiveMessages>())
            {
                if (liv == actor)
                    continue;
                if (!includeTarget && liv == target)
                    continue;
                //TODO Check if they are awake first
                IMessage msg = MessageFormatter.Instance.Format(liv, actor, messageID + (liv == target ? ".target" : ".others"), formatString, target, dct);
                if (msg != null)
                    liv.Write(msg);
            }
        }

        public static void WriteLine(this IReceiveMessages receiver, string messageName, string messageText)
        {
            Write(receiver, messageName, messageText + Environment.NewLine);
        }

        public static void Write(this IReceiveMessages receiver, string messageName, string messageText)
        {
            Write(receiver, new MessageName(messageName), messageText);
        }

        public static void WriteLine(this IReceiveMessages receiver, MessageName name, string messageText)
        {
            Write(receiver, name, messageText + Environment.NewLine);
        }

        public static void Write(this IReceiveMessages receiver, MessageName name, string messageText)
        {
            MessageType type = MessageType.Information;
            if (name.Namespace.Contains("error"))
                type = MessageType.PlayerError;
            Write(receiver, type, name, messageText);
        }

        public static void WriteLine(this IReceiveMessages receiver, MessageType type, string messageName, string messageText)
        {
            Write(receiver, type, messageName, messageText + Environment.NewLine);
        }

        public static void Write(this IReceiveMessages receiver, MessageType type, string messageName, string messageText)
        {
            Write(receiver, type, new MessageName(messageName), messageText);
        }

        public static void WriteLine(this IReceiveMessages receiver, MessageType type, MessageName name, string messageText)
        {
            Write(receiver, type, name, messageText + Environment.NewLine);
        }

        public static void Write(this IReceiveMessages receiver, MessageType type, MessageName name, string messageText)
        {
            if (messageText.Contains("${") && receiver is Living)
            {
                Living actor = receiver as Living;
                StringMessage msg = MessageFormatter.Instance.Format(actor, actor, "", messageText);
                msg.MessageType = type;
                msg.Name = name;
                receiver.Write(msg);
            }
            else
            {
                receiver.Write(new StringMessage(type, name, messageText));
            }
        }
    }
}

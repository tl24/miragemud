﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mirage.Game.Communication;

namespace Mirage.Game.World
{
    public static class MessageSendingExtensions
    {

        /// <summary>
        /// Sends a message to the character only
        /// </summary>
        /// <param name="actor">the actor and recipient</param>
        /// <param name="messageID">The message ID of the message, ".self" will be appended to it</param>
        /// <param name="formatString">Format of the message string or template</param>
        /// <param name="target">The target parameter</param>
        /// <param name="args">an optional list of arguments used in the format string</param>
        public static void ToSelf(this Living actor, string messageID, string formatString, Living target, params object[] args)
        {
            IMessage msg = MessageFormatter.Instance.Format(actor, actor, messageID + ".self", formatString, target, FormatArgs(args));
            if (msg != null)
                actor.Write(msg);
        }

        /// <summary>
        /// Sends a message to the target only
        /// </summary>
        /// <param name="actor">the actor</param>
        /// <param name="messageID">The message ID of the message, ".self" will be appended to it</param>
        /// <param name="formatString">Format of the message string or template</param>
        /// <param name="target">The target and recipient</param>
        /// <param name="args">an optional list of arguments used in the format string</param>
        public static void ToTarget(this Living actor, string messageID, string formatString, Living target, params object[] args)
        {
            IMessage msg = MessageFormatter.Instance.Format(target, actor, messageID + ".target", formatString, target, FormatArgs(args));
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


        public static void ToBystanders(this Living actor, string messageID, string formatString, Living target, params object[] args)
        {
            ToRoomImpl(actor, messageID, formatString, target, false, args);
        }

        public static void ToRoom(this Living actor, string messageID, string formatString)
        {
            ToRoom(actor, messageID, formatString, null);
        }

        public static void ToRoom(this Living actor, string messageID, string formatString, Living target, params object[] args)
        {
            ToRoomImpl(actor, messageID, formatString, target, true, args);
        }

        private static void ToRoomImpl(this Living actor, string messageID, string formatString, Living target, bool includeTarget, params object[] args)
        {
            var dct = FormatArgs(args);
            if (actor == null)
                return;
            if (actor.Container == null)
                return;
            foreach(Living liv in actor.Container.Contents<Living>()) {
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

        public static void Write(this IReceiveMessages receiver, string messageName, string messageText)
        {
            Write(receiver, new MessageName(messageName), messageText);
        }

        public static void Write(this IReceiveMessages receiver, MessageName name, string messageText)
        {
            MessageType type = MessageType.Information;
            if (name.Namespace.Contains("error"))
                type = MessageType.PlayerError;
            Write(receiver, type, name, messageText);
        }

        public static void Write(this IReceiveMessages receiver, MessageType type, string messageName, string messageText)
        {
            Write(receiver, type, new MessageName(messageName), messageText);
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
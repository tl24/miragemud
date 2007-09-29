using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace Mirage.Core.Communication
{
    /// <summary>
    /// Factory class for creating well known message types
    /// </summary>
    public static class MessageFactory
    {
        private static IDictionary<string, IMessage> _messages;

        static MessageFactory()
        {
            _messages = new Dictionary<string, IMessage>(StringComparer.CurrentCultureIgnoreCase);
            _messages[EchoOn] = new StringMessage(MessageType.UIControl, Namespaces.System, EchoOn, "\x1B[0m");
            _messages[EchoOff] = new StringMessage(MessageType.UIControl, Namespaces.System, EchoOff, "\x1B[0;30;40m");
            _messages[NoOp] = new StringMessage(MessageType.UIControl, Namespaces.System, NoOp, "");
        }

        /// <summary>
        /// Returns the given message constant
        /// </summary>
        /// <param name="key">the key of the message</param>
        /// <returns>the message</returns>
        public static IMessage GetMessage(string key)
        {
            return _messages[key];
        }

        public const string EchoOn = "EchoOn";
        public const string EchoOff = "EchoOff";
        public const string NoOp = "NoOp";

    }
}

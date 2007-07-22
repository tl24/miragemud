using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace Mirage.Communication
{
    /// <summary>
    /// Factory class for creating well known message types
    /// </summary>
    public static class MessageFactory
    {
        private static IDictionary<string, Message> _messages;

        static MessageFactory()
        {
            _messages = new Dictionary<string, Message>(StringComparer.CurrentCultureIgnoreCase);
            _messages[EchoOn] = new StringMessage(MessageType.UIControl, Namespaces.System, EchoOn, "\x1B[0m");
            _messages[EchoOff] = new StringMessage(MessageType.UIControl, Namespaces.System, EchoOff, "\x1B[0;30;40m");
        }

        /// <summary>
        /// Returns the given message constant
        /// </summary>
        /// <param name="key">the key of the message</param>
        /// <returns>the message</returns>
        public static Message GetMessage(string key)
        {
            return _messages[key];
        }

        public const string EchoOn = "EchoOn";
        public const string EchoOff = "EchoOff";

    }
}

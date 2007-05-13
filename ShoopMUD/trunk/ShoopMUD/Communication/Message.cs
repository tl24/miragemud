using System;
using System.Collections.Generic;
using System.Text;

namespace Shoop.Communication
{
    public enum MessageType
    {
        Information,
        Communication,
        Multiple,
        Prompt,
        SystemError,
        PlayerError,
        UIControl
    }

    public enum MessageTarget
    {
        Self,
        Other
    }

    /// <summary>
    /// Base class for messages that are sent to the client
    /// </summary>
    public class Message
    {
        private MessageType _messageType;
        private string _name;

        public Message(MessageType messageType, string name)
        {
            this._messageType = messageType;
            this._name = name;
        }

        /// <summary>
        /// The general type of message.  This should be a broad
        /// category that the client can use to figure out how to
        /// process the message.
        /// </summary>
        public MessageType MessageType
        {
            get { return this._messageType; }
            set { this._messageType = value; }
        }

        /// <summary>
        /// The name of the message, every message should have a name.
        /// </summary>
        public string Name
        {
            get { return this._name; }
            set { this._name = value; }
        }
    }
}

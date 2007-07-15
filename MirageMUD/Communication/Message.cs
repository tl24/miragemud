using System;
using System.Collections.Generic;
using System.Text;

namespace Mirage.Communication
{
    public enum MessageType
    {
        /// <summary>
        /// Information to the player
        /// </summary>
        Information,

        /// <summary>
        /// Channel communication
        /// </summary>
        Communication,
        /// <summary>
        /// Message contains multiple parts
        /// </summary>
        Multiple,
        /// <summary>
        /// Message contains a prompt for input from the player
        /// </summary>
        Prompt,

        /// <summary>
        /// problem with a system error
        /// </summary>
        SystemError,
        /// <summary>
        /// Validation error for the player
        /// </summary>
        PlayerError,
        /// <summary>
        /// Message contains control codes or escape sequences for controlling display on the client
        /// such as line break info.
        /// </summary>
        UIControl,
        /// <summary>
        /// Confirmation that a player completed a command successfully
        /// </summary>
        Confirmation,

        /// <summary>
        /// Message contains a notification of an event
        /// </summary>
        Notification,

        /// <summary>
        /// Message contains a data object
        /// </summary>
        Data
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

        public Message()
        {
        }

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

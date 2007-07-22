using System;
using System.Collections.Generic;
using System.Text;

namespace Mirage.Communication
{
    public enum MessageType
    {
        /// <summary>
        /// Message type is unknown
        /// </summary>
        Unknown,

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
        private Uri _namespace;
        private string _name;

        public Message()
        {
            _namespace = Namespaces.Root;
        }

        public Message(MessageType messageType, string name) : this(messageType, Namespaces.Root, name)
        {
        }

        public Message(MessageType messageType, Uri Namespace, string name)
        {
            this._messageType = messageType;
            this._name = name;
            this._namespace = Namespace;
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
        /// The Name of the message, every message should have an Name.
        /// </summary>
        public string Name
        {
            get { return this._name; }
            set { this._name = value; }
        }

        /// <summary>
        /// The fully qualified name which includes the namespace and name
        /// </summary>
        public string QualifiedName
        {
            get
            {
                Uri nmspace = Namespace;
                if (nmspace == null)
                    nmspace = Namespaces.Root;

                return new Uri(nmspace, _name).ToString();
            }
        }
        /// <summary>
        /// The namespace for the message
        /// The Namespace class should be used for constants,
        /// otherwise the format is:
        /// msg:/name/name
        /// </summary>
        public Uri Namespace
        {
            get { return this._namespace; }
            set { this._namespace = value; }
        }

        /// <summary>
        /// Test to see if this message is of a certain type.
        /// </summary>
        /// <param name="type">the type to test</param>
        /// <returns>true if same type</returns>
        public bool IsMatch(MessageType type)
        {
            return IsMatch(type, null, null);
        }

        /// <summary>
        /// Test to see if this message's namespace matches the desired namespace
        /// or if the desired namespace is a base of this message.
        /// </summary>
        /// <param name="baseNamespace">namespace to test</param>
        /// <returns>true if match</returns>
        public bool IsMatch(Uri baseNamespace)
        {
            return IsMatch(MessageType.Unknown, baseNamespace, null);
        }

        /// <summary>
        /// Check to see if this message matches on namespace and name
        /// </summary>
        /// <param name="baseNamespace">namespace to check</param>
        /// <param name="name">name to check</param>
        /// <returns>true if match</returns>
        public bool IsMatch(Uri baseNamespace, string name)
        {
            return IsMatch(MessageType.Unknown, baseNamespace, name);
        }

        /// <summary>
        /// Checks to see if this message matches an expected set of 
        /// parameters.  Any null or empty parameters are ignored.
        /// </summary>
        /// <param name="type">expected type</param>
        /// <param name="baseNamespace">expected namespace</param>
        /// <param name="name">expected name</param>
        /// <returns>true if it matches</returns>
        public bool IsMatch(MessageType type, Uri baseNamespace, string name)
        {
            if (type != MessageType.Unknown
                && MessageType != type)
                return false;
        
            if (baseNamespace != null
                && name != null
                && name != string.Empty) {

                if (!(new Uri(baseNamespace, name)).Equals(QualifiedName))
                    return false;

            } else if (baseNamespace != null) {
                if (!baseNamespace.IsBaseOf(Namespace) && !baseNamespace.Equals(QualifiedName))
                    return false;
            } else if (name != null) {
                if (name != this.Name)
                    return false;
            }
            return true;
        }

        public override bool Equals(object obj)
        {
            if (!base.Equals(obj) && obj is Message)
            {
                Message other = (Message)obj;
                return IsMatch(other.MessageType, other.Namespace, other.Name);
            }
            return false;
        }
    }
}

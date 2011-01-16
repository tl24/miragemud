using System;
using System.Collections.Generic;
using System.Text;

namespace Mirage.Core.Communication
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
        /// Confirmation that a player completed a command successfully
        /// </summary>
        Confirmation,
         
        /// <summary>
        /// Message contains a data object
        /// </summary>
        Data,

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
    public class Message : IMessage
    {
        private MessageType _messageType;
        private MessageName _name;
        private IDictionary<string, object> _parameters;

        public Message() : this(MessageType.Unknown, MessageName.Empty)
        {
        }

        public Message(MessageType messageType, string name) : this(messageType, new MessageName(name))
        {
        }

        public Message(MessageType messageType, MessageName Name)
            : this(messageType, Name, new Dictionary<string, object>())
        {
        }

        public Message(MessageType messageType, MessageName Name, IDictionary<string, object> parameters)
        {
            this._messageType = messageType;
            this._name = Name;
            this._parameters = parameters;
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
        public MessageName Name
        {
            get { return this._name; }
            set { this._name = value; }
        }

        /// <summary>
        /// The replacement parameters for the template
        /// </summary>
        public IDictionary<string, object> Parameters
        {
            get { return this._parameters; }
            set
            {
                if (value != null)
                    this._parameters = new Dictionary<string, object>(value);
                else
                    this._parameters.Clear();
            }
        }

        /// <summary>
        /// Gets and sets parameters for the template
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public object this[string key]
        {
            get { return Parameters[key]; }
            set { Parameters[key] = value; }
        }

        /// <summary>
        /// Test to see if this message is of a certain type.
        /// </summary>
        /// <param name="type">the type to test</param>
        /// <returns>true if same type</returns>
        public bool IsMatch(MessageType type)
        {
            return type == MessageType.Unknown || type == this.MessageType;
        }

        public bool IsMatch(MessageName name)
        {
            return name.Equals(this.Name);
        }
        /// <summary>
        /// Test to see if this message's namespace matches the desired namespace
        /// or if the desired namespace is a base of this message.
        /// </summary>
        /// <param name="baseNamespace">namespace to test</param>
        /// <returns>true if match</returns>
        public bool IsMatch(string baseNamespace)
        {
            return this.Name.IsPartOfNamespace(baseNamespace) || this.Name.IsSameAs(baseNamespace);
        }

        /// <summary>
        /// Check to see if this message matches on namespace and name
        /// </summary>
        /// <param name="baseNamespace">namespace to check</param>
        /// <param name="name">name to check</param>
        /// <returns>true if match</returns>
        public bool IsMatch(string baseNamespace, string name)
        {
            return IsMatch(new MessageName(baseNamespace, name));
        }

        public override bool Equals(object obj)
        {
            if (!base.Equals(obj) && obj is IMessage)
            {
                IMessage other = (IMessage)obj;
                return other.MessageType == this.MessageType && other.Name.Equals(this.Name);
            }
            return false;
        }

        public virtual bool CanTransferMessage
        {
            get { return true; }
        }

        public virtual IMessage GetTransferMessage()
        {
            return this;
        }

        public virtual string Render()
        {
            return this.ToString();
        }

        public IMessage Copy()
        {
            IMessage result = MakeCopy();
            if (result.GetType() != this.GetType())
                throw new Exception("Incorrect copy procedure, returned object is not of the same type it was copied from");
            return result;
        }

        protected virtual IMessage MakeCopy()
        {
            return this;
        }
    }
}

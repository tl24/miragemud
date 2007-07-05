using System;
using System.Collections.Generic;
using System.Text;

namespace Mirage.Communication
{
    /// <summary>
    /// Simple message type that contains a formatted string
    /// </summary>
    public class StringMessage : Message
    {
       private string _messageString;

        public StringMessage()
        {
        }

        public StringMessage(MessageType messageType, string name, string message)
            : base(messageType, name)
        {
            this._messageString = message;
        }

        public override string ToString() {
            return _messageString;
        }

        /// <summary>
        /// The message string to send to the text client
        /// </summary>
        public string MessageString
        {
            get { return this._messageString; }
            set { this._messageString = value; }
        }

    }
}

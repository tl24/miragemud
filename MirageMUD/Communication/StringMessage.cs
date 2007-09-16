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
       private string _text;

        public StringMessage()
        {
        }

        public StringMessage(MessageType messageType, string name, string text)
            : base(messageType, name)
        {
            this._text = text;
        }

        public StringMessage(MessageType messageType, Uri Namespace, string name, string text)
            : base(messageType, Namespace, name)
        {
            this._text = text;
        }

        public override string ToString() {
            return RenderMessage();
        }

        public override string RenderMessage()
        {
            return _text;
        }
        /// <summary>
        /// The message string to send to the text client
        /// </summary>
        public string Text
        {
            get { return this._text; }
            set { this._text = value; }
        }

    }
}

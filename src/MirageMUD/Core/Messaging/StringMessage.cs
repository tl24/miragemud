
using JsonExSerializer;
namespace Mirage.Core.Messaging
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

        public StringMessage(string name, string text)
            : base(name)
        {
            this._text = text;
        }

        public StringMessage(MessageType messageType, string name, string text)
            : base(messageType, name)
        {
            this._text = text;
        }

        public StringMessage(MessageType messageType, MessageName name, string text)
            : base(messageType, name)
        {
            this._text = text;
        }

        public override string ToString() {
            return Render();
        }

        public override string Render()
        {
            return _text;
        }
        /// <summary>
        /// The message string to send to the text client
        /// </summary>
        //[ConstructorParameter]
        public string Text
        {
            get { return this._text; }
            set { this._text = value; }
        }

    }
}

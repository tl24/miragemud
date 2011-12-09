using Mirage.Game.World.Query;

namespace Mirage.Game.Communication
{
    /// <summary>
    /// Message type for sending data to the client
    /// </summary>
    public class DataMessage : Message
    {
        private string _itemUri;
        private object _data;

        public DataMessage()
        {
            this.MessageType = MessageType.Data;
        }

        public DataMessage(string Namespace, string name, object data)
            : base(MessageType.Data, new MessageName(Namespace, name))
        {
            if (_data is IUri)
                this._itemUri = ((IUri) _data).FullUri;
            this._data = data;
        }

        public DataMessage(string Namespace, string name, string itemUri, object data)
            : base(MessageType.Data, new MessageName(Namespace, name))
        {
            this._itemUri = itemUri;
            this._data = data;
        }

        public DataMessage(string Namespace, string name)
            : base(MessageType.Data, new MessageName(Namespace, name))
        {
        }

        public object Data
        {
            get { return this._data; }
            set { this._data = value; }
        }

        public string ItemUri
        {
            get { return this._itemUri; }
            set { this._itemUri = value; }
        }

        protected override IMessage MakeCopy()
        {
            return new DataMessage();
        }
    }
}

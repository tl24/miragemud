using System;
using System.Collections.Generic;
using System.Text;

namespace Mirage.Communication
{
    /// <summary>
    /// Message type for sending data to the client
    /// </summary>
    public class DataMessage : Message
    {
        private object _data;

        public DataMessage()
        {
            this.MessageType = MessageType.Data;
        }

        public DataMessage(Uri Namespace, string name, object data)
            : base(MessageType.Data, Namespace, name)
        {
            this._data = data;
        }

        public DataMessage(Uri Namespace, string name)
            : base(MessageType.Data, Namespace, name)
        {
        }

        public object Data
        {
            get { return this._data; }
            set { this._data = value; }
        }

    }
}

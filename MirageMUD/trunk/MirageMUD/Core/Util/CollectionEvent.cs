using System;
using System.Collections.Generic;
using System.Text;

namespace Mirage.Core.Util
{
    public enum CollectionEventType {
        Add,
        Update,
        Remove,
        AddRange,
        UpdateRange,
        RemoveRange,
        Cleared
    }

    public class CollectionEventArgs : EventArgs
    {
        private object _keys;
        private CollectionEventType _eventType;
        public CollectionEventArgs(CollectionEventType eventType, object keys)
        {
            this._keys = keys;
            this._eventType = eventType;
        }

        public object Keys
        {
            get { return this._keys; }
        }

        public CollectionEventType EventType
        {
            get { return this._eventType; }
        }

    }
}

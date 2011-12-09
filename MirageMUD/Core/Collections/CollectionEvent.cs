using System;

namespace Mirage.Core.Collections
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

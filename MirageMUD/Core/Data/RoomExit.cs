using System;
using System.Collections.Generic;
using System.Text;
using Mirage.Core.Data.Query;
using Mirage.Core.Data.Attribute;
using JsonExSerializer;
using System.Collections;

namespace Mirage.Core.Data
{
    public enum DirectionType {
        North,
        East,
        West,
        South,
        Up,
        Down
    }

    public class RoomExit : Thing, IAttributable
    {
        private DirectionType _direction;
        private string _toRoomURI;
        private Room _targetRoom;
        private Room _parentRoom;

        public RoomExit()
        {
        }

        public RoomExit(DirectionType direction, string targetRoomURI, Room parentRoom)
        {
            this._direction = direction;
            this._toRoomURI = targetRoomURI;
            this._parentRoom = parentRoom;
        }

        public RoomExit(DirectionType direction, string targetRoomURI) : this(direction, targetRoomURI, null) {
        }

        [Editor(Priority=1)]
        public Mirage.Core.Data.DirectionType Direction
        {
            get { return this._direction; }
            set { this._direction = value; }
        }

        public string ToRoomURI
        {
            get { return this._toRoomURI; }
            set { this._toRoomURI = value; }
        }

        [EditorParent(2)]
        public Room ParentRoom
        {
            get { return this._parentRoom; }
            set { this._parentRoom = value; }
        }

        public Room TargetRoom
        {
            get {
                if (_targetRoom == null) {
                    QueryManager queryManager = new QueryManager();
                    if (_toRoomURI.StartsWith("/") || _toRoomURI.StartsWith("Areas")) {
                        // absolute link
                        _targetRoom = (Room)queryManager.Find(_toRoomURI);
                    } else {
                        // relative
                        _targetRoom = (Room)queryManager.Find(_parentRoom.Area, "Rooms/" + _toRoomURI);
                    }
                }
                return _targetRoom;
            }
        }

        public override string ToString()
        {
            return _direction.ToString();
        }

        #region IAttributable Members

        public bool AddAttribute(object attribute)
        {
            _attributes.Add(attribute);
            return true;
        }


        public bool RemoveAttribute(Type t)
        {
            object o = TryGetAttribute(t);
            if (o != null)
            {
                _attributes.Remove(o);
            }
            return o != null;
        }

        [EditorTypeSelector(typeof(BaseAttribute), "Mirage.Core.Data.Attribute")]
        public ArrayList Attributes
        {
            get { return _attributes; }
            set { _attributes = value; }
        }
        #endregion
    }
}

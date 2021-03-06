using System;
using System.Collections;
using Mirage.Core.Extensibility;
using Mirage.Game.World.Attribute;
using Mirage.Game.World.Query;

namespace Mirage.Game.World
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
        public DirectionType Direction
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
                    MudWorld world = MudFactory.GetObject<MudWorld>();
                    if (_toRoomURI.StartsWith("/") || _toRoomURI.StartsWith("Areas")) {
                        // absolute link
                        _targetRoom = (Room)world.ResolveUri(_toRoomURI);
                    } else {
                        // relative
                        _targetRoom = (Room)world.ResolveUri(_parentRoom.Area, "Rooms/" + _toRoomURI);
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

        [EditorTypeSelector(typeof(BaseAttribute), "Mirage.Game.World.Attribute")]
        public ArrayList Attributes
        {
            get { return _attributes; }
            set { _attributes = value; }
        }
        #endregion
    }
}

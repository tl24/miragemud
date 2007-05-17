using System;
using System.Collections.Generic;
using System.Text;

namespace Shoop.Data
{
    public enum DirectionType {
        North,
        East,
        West,
        South,
        Up,
        Down
    }

    public class RoomExit
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

        public Shoop.Data.DirectionType Direction
        {
            get { return this._direction; }
            set { this._direction = value; }
        }

        public string ToRoomURI
        {
            get { return this._toRoomURI; }
            set { this._toRoomURI = value; }
        }

        public Shoop.Data.Room ParentRoom
        {
            get { return this._parentRoom; }
            set { this._parentRoom = value; }
        }

        public Room TargetRoom
        {
            get {
                if (_targetRoom == null) {
                    if (_toRoomURI.StartsWith("/") || _toRoomURI.StartsWith("Areas")) {
                        // absolute link
                        _targetRoom = (Room) GlobalLists.GetInstance().Find(_toRoomURI);
                    } else {
                        // relative
                        _targetRoom = (Room) _parentRoom.Area.Find("Rooms/" + _toRoomURI);
                    }
                }
                return _targetRoom;
            }
        }

        public override string ToString()
        {
            return _direction.ToString();
        }
    }
}

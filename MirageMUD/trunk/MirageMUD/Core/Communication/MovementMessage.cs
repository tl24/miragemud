using System;
using System.Collections.Generic;
using System.Text;

namespace Mirage.Core.Communication
{
    public class MovementMessage : StringMessage
    {
        private string _direction;
        private MovementType _movementType;
        private string _actorName;

        public enum MovementType {
            Arrival,
            Departure
        }

        public MovementMessage()
        {
        }

        public MovementMessage(string direction, MovementType movementType, string actorName)
            : base(MessageType.Notification, Namespaces.Movement, direction, "Animate.Movement")
        {
            this._direction = direction ?? "";
            this._movementType = movementType;
            this._actorName = actorName ?? "";
        }

        public string Direction
        {
            get { return this._direction; }
            set { this._direction = value; }
        }

        public string ActorName
        {
            get { return this._actorName; }
            set { this._actorName = value; }
        }

        public MovementType MoveType
        {
            get { return this._movementType; }
            set { this._movementType = value; }
        }

        public override string Render()
        {
            return ToString();
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            if (_movementType == MovementType.Arrival)
            {
                sb.Append(_actorName == string.Empty ? "Someone" : _actorName);
                sb.Append(" has arrived.\r\n");
            }
            else
            {
                sb.Append(_actorName == string.Empty ? "Someone" : _actorName);
                if (_direction == string.Empty)
                {
                    sb.Append(" has left the room.\r\n");
                }
                else
                {
                    if (_direction == "up" || _direction == "down") {
                        sb.Append(" leaves ").Append(_direction);
                    } else {
                        sb.Append(" leaves to the ").Append(_direction);
                    }
                    sb.Append(".\r\n");
                }
            }
            return sb.ToString();
        }

        protected override IMessage MakeCopy()
        {
            throw new Exception("Method not implemented");
        }
    }
}

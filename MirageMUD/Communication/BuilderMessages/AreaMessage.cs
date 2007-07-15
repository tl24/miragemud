using System;
using System.Collections.Generic;
using System.Text;
using Mirage.Data;

namespace Mirage.Communication.BuilderMessages
{
    public class AreaMessage : Message
    {
        private Area _area;

        public AreaMessage() : this(null) { }

        public AreaMessage(Area area)
            : base(MessageType.Data, "Area.Get")
        {
            this._area = area;
        }

        public Mirage.Data.Area Area
        {
            get { return this._area; }
            set { this._area = value; }
        }
    }
}

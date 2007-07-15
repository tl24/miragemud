using System;
using System.Collections.Generic;
using System.Text;

namespace Mirage.Communication.BuilderMessages
{
    public class AreaListMessage : Message
    {
        private List<string> _areas;

        public AreaListMessage()
            : base(MessageType.Data, "Area.List")
        {
            _areas = new List<string>();
        }

        public AreaListMessage(List<string> areas)
            : base(MessageType.Data, "Area.List")
        {
            _areas = areas;
        }

        public List<string> Areas
        {
            get { return this._areas; }
            set { this._areas = value; }
        }


    }
}

using System;
using System.Collections.Generic;
using System.Text;
using Mirage.Core.Data.Query;
using Mirage.Core.Data;

namespace Mirage.Stock.Data
{
    public class MobReset : ResetBase
    {
        private string _roomUri;
        private Room _targetRoom;

        public MobReset()
        {
        }

        public string RoomUri
        {
            get { return this._roomUri; }
            set
            {
                this._roomUri = value;
                _targetRoom = null;
            }
        }

        public Room GetRoom() 
        {
            if (_targetRoom == null)
            {
                IQueryManager queryManager = MudFactory.GetObject<IQueryManager>();

                // absolute link
                _targetRoom = (Room)queryManager.Find(_roomUri);

            }
            return _targetRoom;

        }
    }
}

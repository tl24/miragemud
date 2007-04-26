using System;
using System.Collections.Generic;
using System.Text;
using Shoop.Data.Query;

namespace Shoop.Data
{
    public class Area : BaseData
    {
    
        private string _name;
        private string _description;
        private SortedDictionary<string, Room> _rooms;

        public Area()
        {
            _rooms = new SortedDictionary<string, Room>();
            _uriProperties["rooms"] = new QuerySortedDictionaryAdapter<Room>("rooms", _rooms);
        }

        public SortedDictionary<string, Room> Rooms
        {
            get { return this._rooms; }
            set { this._rooms = value; }
        }

        public string Name
        {
            get { return this._name; }
            set { this._name = value; }
        }

        public string Description
        {
            get { return this._description; }
            set { this._description = value; }
        }
    }
}

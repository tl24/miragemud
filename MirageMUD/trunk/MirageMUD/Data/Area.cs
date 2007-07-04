using System;
using System.Collections.Generic;
using System.Text;
using Mirage.Data.Query;

namespace Mirage.Data
{
    public class Area : BaseData, IViewable
    {
        private string _title;
        private string _shortDescription;
        private string _longDescription;
        private IDictionary<string, Room> _rooms;

        public Area()
        {
            Rooms = new Dictionary<string, Room>();            
        }

        public IDictionary<string, Room> Rooms
        {
            get { return this._rooms; }
            set { 
                this._rooms = value;
                _uriChildCollections["Rooms"] = new BaseData.ChildCollectionPair(_rooms, QueryHints.UriKeyedDictionary | QueryHints.UniqueItems);
            }
        }

        public string LongDescription
        {
            get { return this._longDescription; }
            set { this._longDescription = value; }
        }

        public string ShortDescription
        {
            get { return this._shortDescription; }
            set { this._shortDescription = value; }
        }

        public string Title
        {
            get { return this._title; }
            set { this._title = value; }
        }

        public override string FullUri
        {
            get
            {
                return "Areas/" + this.Uri;
            }
        }
    }
}

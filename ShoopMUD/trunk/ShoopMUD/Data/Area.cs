using System;
using System.Collections.Generic;
using System.Text;
using Shoop.Data.Query;

namespace Shoop.Data
{
    public class Area : BaseData, IViewable
    {
        private string _title;
        private string _shortDescription;
        private string _longDescription;
        private IDictionary<string, Room> _rooms;

        public Area()
        {
            _rooms = new Dictionary<string, Room>();
            _uriChildCollections["Rooms"] = new BaseData.ChildCollectionPair(_rooms, QueryCollectionFlags.UriKeyedDictionary | QueryCollectionFlags.UniqueItems);
        }

        public IDictionary<string, Room> Rooms
        {
            get { return this._rooms; }
            set { this._rooms = value; }
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

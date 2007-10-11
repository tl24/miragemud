using System;
using System.Collections.Generic;
using System.Text;
using Mirage.Core.Data.Query;
using JsonExSerializer;
using System.Configuration;
using Mirage.Core.Data;

namespace Mirage.Stock.Data
{
    public class Area : BaseData, IViewable, IArea
    {
        private string _title;
        private string _shortDescription;
        private string _longDescription;
        private IDictionary<string, Room> _rooms;
        private bool _isDirty;

        public Area()
        {
            Rooms = new Dictionary<string, Room>(StringComparer.CurrentCultureIgnoreCase);            
        }

        [EditorCollection(typeof(Room))]
        public IDictionary<string, Room> Rooms
        {
            get { return this._rooms; }
            set { 
                this._rooms = value;
                _uriChildCollections["Rooms"] = new BaseData.ChildCollectionPair(_rooms, QueryHints.UriKeyedDictionary | QueryHints.UniqueItems);
            }
        }

        [Editor(Priority = 3)]
        public string Title
        {
            get { return this._title; }
            set { this._title = value; }
        }

        [Editor(Priority = 4)]
        public string ShortDescription
        {
            get { return this._shortDescription; }
            set { this._shortDescription = value; }
        }

        [Editor(Priority = 5, EditorType="Multiline")]
        public string LongDescription
        {
            get { return this._longDescription; }
            set { this._longDescription = value; }
        }

        [JsonExIgnore]
        public bool IsDirty
        {
            get { return this._isDirty; }
            set { this._isDirty = value; }
        }

        public override string FullUri
        {
            get
            {
                return "Areas/" + this.Uri;
            }
        }

        /// <summary>
        /// Copies the contents, players, mobiles, objects to the
        /// new area
        /// </summary>
        /// <param name="newArea">the new area</param>
        public void CopyTo(Area newArea)
        {
            Room defaultRoom = (Room)new QueryManager().Find(ConfigurationManager.AppSettings["default.room"]);
            if (defaultRoom.Area.Uri == this.Uri)
            {
                // check to see if it still exists
                if (newArea.Rooms.ContainsKey(defaultRoom.Uri))
                {
                    // it does, use the new room
                    defaultRoom = newArea.Rooms[defaultRoom.Uri];
                }
                else
                {
                    // it doesn't, pick an arbitrary room to move to
                    foreach (Room r in newArea.Rooms.Values)
                    {
                        defaultRoom = r;
                        break;
                    }
                }            
            }

            foreach (Room room in Rooms.Values)
            {
                if (newArea.Rooms.ContainsKey(room.Uri))
                {
                    room.CopyTo(newArea.Rooms[room.Uri]);
                }
                else
                {
                    room.CopyTo(defaultRoom);
                }
            }
        }
    }
}

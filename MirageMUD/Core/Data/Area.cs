using System;
using System.Collections.Generic;
using System.Text;
using Mirage.Core.Data.Query;
using JsonExSerializer;
using System.Configuration;
using Mirage.Core.Data;

namespace Mirage.Core.Data
{
    public class Area : ViewableBase, IArea
    {
        private IDictionary<string, Room> _rooms;
        private IDictionary<string, MobTemplate> _mobiles;

        private bool _isDirty;

        public Area()
        {
            _rooms = new Dictionary<string, Room>(StringComparer.CurrentCultureIgnoreCase);
            _uriChildCollections["Rooms"] = new BaseData.ChildCollectionPair(_rooms, QueryHints.UriKeyedDictionary | QueryHints.UniqueItems);
            _mobiles = new Dictionary<string, MobTemplate>(StringComparer.CurrentCultureIgnoreCase);
            _uriChildCollections["Mobiles"] = new BaseData.ChildCollectionPair(_mobiles, QueryHints.UriKeyedDictionary | QueryHints.UniqueItems);
        }

        [EditorCollection(typeof(Room))]
        [JsonExProperty]
        public IDictionary<string, Room> Rooms
        {
            get { return this._rooms; }
            /*
            set { 
                this._rooms = value;
                _uriChildCollections["Rooms"] = new BaseData.ChildCollectionPair(_rooms, QueryHints.UriKeyedDictionary | QueryHints.UniqueItems);
            }
             */ 
        }

        [EditorCollection(typeof(MobTemplate))]
        [JsonExProperty]
        public IDictionary<string, MobTemplate> Mobiles
        {
            get { return this._mobiles; }
            /*
            set
            {
                this._mobiles = value;
                _uriChildCollections["Mobiles"] = new BaseData.ChildCollectionPair(_mobiles, QueryHints.UriKeyedDictionary | QueryHints.UniqueItems);
            }
             */ 
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
            Room defaultRoom = (Room)MudFactory.GetObject<IQueryManager>().Find(ConfigurationManager.AppSettings["default.room"]);
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

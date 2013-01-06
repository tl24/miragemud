using System;
using System.Collections.Generic;
using System.Configuration;
using JsonExSerializer;
using Mirage.Game.World.Query;

namespace Mirage.Game.World
{
    public class Area : ViewableBase, IArea
    {
        public Area()
        {
            Rooms = new Dictionary<string, Room>(StringComparer.CurrentCultureIgnoreCase);
            Mobiles = new Dictionary<string, MobTemplate>(StringComparer.CurrentCultureIgnoreCase);
        }

        [EditorCollection(typeof(Room))]
        [JsonExProperty]
        public IDictionary<string, Room> Rooms { get; private set; }

        [EditorCollection(typeof(MobTemplate))]
        [JsonExProperty]
        public IDictionary<string, MobTemplate> Mobiles{ get; private set; }

        [JsonExIgnore]
        public bool IsDirty { get; set; }

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
            Room defaultRoom = (Room)MudFactory.GetObject<MudWorld>().ResolveUri(ConfigurationManager.AppSettings["default.room"]);
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

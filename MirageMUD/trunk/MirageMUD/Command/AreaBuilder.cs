using System;
using System.Collections.Generic;
using System.Text;
using Mirage.Communication.BuilderMessages;
using Mirage.Communication;
using Mirage.Data;
using Mirage.IO;
using Mirage.IO.Serialization;
using Mirage.Data.Query;

namespace Mirage.Command
{
    /// <summary>
    /// Type of edit occurring
    /// </summary>
    public enum ChangeType
    {
        None,
        Add,
        Edit,
        Delete
    }

    /// <summary>
    /// Contains the area level commands for the builder
    /// </summary>
    [CommandDefaults(
        ClientTypes = new Type[] { typeof(GuiClient) },
        Roles = "builder")]
    public class AreaBuilder
    {
        /// <summary>
        /// Get the top node of the area hierarchy
        /// </summary>
        /// <returns></returns>
        [Command]
        public static IMessage GetWorld()
        {
            return new DataMessage(Namespaces.Area, "World", new World());
        }

        /// <summary>
        /// Retrieves the names of all the areas in the mud
        /// </summary>
        /// <returns>area list</returns>
        [Command]
        public static IMessage GetAreas(string itemUri)
        {
            IDictionary<string, Area> areas = MudRepository.GetInstance().Areas;
            List<string> areaList = new List<string>(areas.Keys);
            return new DataMessage(Namespaces.Area, "AreaList", "Areas", areaList);
        }

        /// <summary>
        /// Adds a newly created area
        /// </summary>
        /// <returns>confirmation</returns>
        [Command]
        public static IMessage UpdateItem(ChangeType changeType, Area area)
        {
            IDictionary<string, Area> areas = MudRepository.GetInstance().Areas;
            switch (changeType)
            {
                case ChangeType.Add:                    
                    areas[area.Uri] = area;
                    area.IsDirty = true;
                    return new UpdateConfirmationMessage(Namespaces.Area, "AreaAdded", area.FullUri, changeType);
                case ChangeType.Edit:
                    //TODO: need to do room contents copy
                    Area old = areas[area.Uri];
                    old.CopyTo(area);
                    areas[area.Uri] = area;

                    //ObjectUpdater.CopyObject(area, dest);
                    area.IsDirty = true;
                    return new UpdateConfirmationMessage(Namespaces.Area, "AreaUpdated", area.FullUri, changeType);
                default:
                    throw new ArgumentException("Invalid changeType: " + changeType);
            }
            
        }

        [Command]
        public static IMessage SaveArea(string areaName)
        {
            IPersistenceManager persister = ObjectStorageFactory.GetPersistenceManager(typeof(Area));
            IDictionary<string, Area> areas = MudRepository.GetInstance().Areas;
            if (areaName == null || areaName == string.Empty || areaName == "all")
            {
                foreach (Area area in areas.Values)
                {
                    if (area.IsDirty)
                        persister.Save(area, area.Uri);
                }
                return new Message(MessageType.Confirmation, Namespaces.Area, "AllAreasSaved");
            }
            else
            {
                Area area = areas[areaName];
                persister.Save(area, area.Uri);
                return new Message(MessageType.Confirmation, Namespaces.Area, "AreaSaved");
            }
        }

        /// <summary>
        /// Retrieves a specific area
        /// </summary>
        /// <param name="builder">the builder player doing the request</param>
        /// <returns>area</returns>
        [Command]
        public static IMessage GetArea(string itemUri)
        {
            Area area = (Area) QueryManager.GetInstance().Find(itemUri);
            return new DataMessage(Namespaces.Area, "Area", itemUri, area);
        }

        /// <summary>
        /// Get the rooms for an area.  The itemUri should be in the form
        /// /Areas/AreaName/Rooms
        /// </summary>
        /// <param name="itemUri">Uri to the rooms collection of an area</param>
        /// <returns>list of rooms</returns>
        [Command]
        public static IMessage GetRooms(string itemUri)
        {
            IDictionary<string, Room> rooms = (IDictionary<string, Room>) QueryManager.GetInstance().Find(itemUri);
            List<string> roomList = new List<string>(rooms.Keys);
            return new DataMessage(Namespaces.Area, "Rooms", itemUri, roomList);
        }

        /// <summary>
        /// Get a given room for an area.  The itemUri should be in the form
        /// /Areas/AreaName/Rooms/RoomName
        /// </summary>
        /// <param name="itemUri">Uri to the room of an area</param>
        /// <returns>room</returns>
        [Command]
        public static IMessage GetRoom(string itemUri)
        {
            Room room = (Room)QueryManager.GetInstance().Find(itemUri);
            return new DataMessage(Namespaces.Area, "Room", itemUri, room);
        }
    }
}

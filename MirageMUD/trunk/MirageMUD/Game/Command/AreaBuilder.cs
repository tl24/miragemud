using System;
using System.Collections.Generic;
using Mirage.Game.Communication;
using Mirage.Game.Communication.BuilderMessages;
using Mirage.Game.IO.Net;
using Mirage.Game.World;
using Mirage.Game.World.Query;
using Mirage.Core.Messaging;
using Mirage.Core.Command;

namespace Mirage.Game.Command
{

    /// <summary>
    /// Contains the area level commands for the builder
    /// </summary>
    [ClientTypesRestriction(typeof(AdvancedClient))]
    [RoleRestriction("builder")]
    public class AreaBuilder : CommandDefaults
    {
        public IAreaRepository AreaRepository { get; set; }

        /// <summary>
        /// Retrieves the names of all the areas in the mud
        /// </summary>
        /// <returns>area list</returns>
        [Command]
        public IMessage GetAreas(string itemUri)
        {
            IDictionary<string, IArea> areas = AreaRepository.Areas;
            List<string> areaList = new List<string>(areas.Keys);
            return new DataMessage(Namespaces.Area, "AreaList", "Areas", areaList);
        }

        /// <summary>
        /// Adds a newly created area
        /// </summary>
        /// <returns>confirmation</returns>
        [Command]
        public IMessage UpdateItem(ChangeType changeType, Area area)
        {
            IDictionary<string, IArea> areas = AreaRepository.Areas;
            switch (changeType)
            {
                case ChangeType.Add:                    
                    areas[area.Uri] = area;
                    area.IsDirty = true;
                    return new UpdateConfirmationMessage("builder.area.AreaAdded", area.FullUri, changeType);
                case ChangeType.Edit:
                    //TODO: need to do room contents copy
                    Area old = (Area) areas[area.Uri];
                    old.CopyTo(area);
                    areas[area.Uri] = area;

                    //ObjectUpdater.CopyObject(area, dest);
                    area.IsDirty = true;
                    return new UpdateConfirmationMessage("builder.area.AreaUpdated", area.FullUri, changeType);
                default:
                    throw new ArgumentException("Invalid changeType: " + changeType);
            }
            
        }

        [Command]
        public IMessage SaveArea(string areaName)
        {
            if (areaName == null || areaName == string.Empty || areaName == "all")
            {
                foreach (Area area in AreaRepository)
                {
                    if (area.IsDirty)
                        AreaRepository.Save(area);
                }
                return new Message(MessageType.Confirmation, "builder.area.AllAreasSaved");
            }
            else
            {
                AreaRepository.Save(areaName);
                return new Message(MessageType.Confirmation, "builder.area.AreaSaved");
            }
        }

        /// <summary>
        /// Retrieves a specific area
        /// </summary>
        /// <param name="builder">the builder player doing the request</param>
        /// <returns>area</returns>
        [Command]
        public IMessage GetArea(string itemUri)
        {
            Area area = (Area) World.ResolveUri(itemUri);
            return new DataMessage(Namespaces.Area, "Area", itemUri, area);
        }

        /// <summary>
        /// Get the rooms for an area.  The itemUri should be in the form
        /// /Areas/AreaName/Rooms
        /// </summary>
        /// <param name="itemUri">Uri to the rooms collection of an area</param>
        /// <returns>list of rooms</returns>
        [Command]
        public IMessage GetRooms(string itemUri)
        {
            IDictionary<string, Room> rooms = (IDictionary<string, Room>)World.ResolveUri(itemUri);
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
        public IMessage GetRoom(string itemUri)
        {
            Room room = (Room)World.ResolveUri(itemUri);
            return new DataMessage(Namespaces.Area, "Room", itemUri, room);
        }
    }
}

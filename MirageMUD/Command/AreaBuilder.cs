using System;
using System.Collections.Generic;
using System.Text;
using Mirage.Communication.BuilderMessages;
using Mirage.Communication;
using Mirage.Data;
using Mirage.IO;
using Mirage.IO.Serialization;

namespace Mirage.Command
{
    /// <summary>
    /// Contains the area level commands for the builder
    /// </summary>
    [CommandDefaults(
        ClientTypes = new Type[] { typeof(GuiClient) },
        Roles = "builder")]
    public class AreaBuilder
    {
        /// <summary>
        /// Retrieves the names of all the areas in the mud
        /// </summary>
        /// <returns>area list</returns>
        [Command]
        public static Message GetAreas()
        {
            IDictionary<string, Area> areas = GlobalLists.GetInstance().Areas;
            List<string> areaList = new List<string>(areas.Keys);
            return new ChildItemsMessage(Namespaces.Area, "AreaList", GlobalLists.GetInstance().FullUri, areaList);
        }

        /// <summary>
        /// Adds a newly created area
        /// </summary>
        /// <returns>confirmation</returns>
        [Command]
        public static Message AddArea(Area newArea)
        {
            IDictionary<string, Area> areas = GlobalLists.GetInstance().Areas;
            areas[newArea.Uri] = newArea;
            newArea.IsDirty = true;
            return new Message(MessageType.Confirmation, Namespaces.Area, "AreaAdded");
        }

        /// <summary>
        /// Updates an existing area
        /// </summary>
        /// <returns>confirmation</returns>
        [Command]
        public static Message UpdateArea(Area updatedArea)
        {
            IDictionary<string, Area> areas = GlobalLists.GetInstance().Areas;
            Area dest = areas[updatedArea.Uri];
            ObjectUpdater.CopyObject(updatedArea, dest);
            dest.IsDirty = true;
            return new Message(MessageType.Confirmation, Namespaces.Area, "AreaUpdated");
        }

        [Command]
        public static Message SaveArea(string areaName)
        {
            IPersistenceManager persister = ObjectStorageFactory.GetPersistenceManager(typeof(Area));
            IDictionary<string, Area> areas = GlobalLists.GetInstance().Areas;
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
        public static Message GetArea(string areaName)
        {
            Area area = GlobalLists.GetInstance().Areas[areaName];
            return new DataMessage(Namespaces.Area, "Area", area);
        }

        [Command]
        public static Message GetAreaRooms(string areaName)
        {
            Area area = GlobalLists.GetInstance().Areas[areaName];
            List<string> roomList = new List<string>(area.Rooms.Keys);
            return new ChildItemsMessage(Namespaces.Area, "AreaRooms", area.FullUri, roomList);
        }
    }
}

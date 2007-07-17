using System;
using System.Collections.Generic;
using System.Text;
using Mirage.Communication.BuilderMessages;
using Mirage.Communication;
using Mirage.Data;
using Mirage.IO;

namespace Mirage.Command
{
    /// <summary>
    /// Contains the area level commands for the builder
    /// </summary>
    public class AreaBuilder
    {
        /// <summary>
        /// Retrieves the names of all the areas in the mud
        /// </summary>
        /// <param name="builder">the builder player doing the request</param>
        /// <returns>area list</returns>
        [Command(
            ClientTypes=new Type[]{typeof(GuiClient)},
            Roles = new string[] {"builder"})]
        public static Message GetAreas()
        {
            IDictionary<string, Area> areas = GlobalLists.GetInstance().Areas;
            List<string> areaList = new List<string>(areas.Keys);
            return new AreaListMessage(areaList);
        }

        /// <summary>
        /// Retrieves a specific area
        /// </summary>
        /// <param name="builder">the builder player doing the request</param>
        /// <returns>area</returns>
        [Command(
            ClientTypes = new Type[] { typeof(GuiClient) },
            Roles = new string[] { "builder" })]
        public static Message GetArea(string name)
        {
            Area area = GlobalLists.GetInstance().Areas[name];
            return new AreaMessage(area);
        }

    }
}

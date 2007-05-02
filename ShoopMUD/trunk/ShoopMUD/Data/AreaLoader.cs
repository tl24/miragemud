using System;
using System.Collections.Generic;
using System.Text;

namespace Shoop.Data
{
    public class AreaLoader
    {
        public void LoadAll()
        {
            GlobalLists globalLists = GlobalLists.GetInstance();
            Area defaultArea = new Area();
            defaultArea.URI = "DefaultArea";
            defaultArea.Title = "Default Area";
            defaultArea.ShortDescription = "This is the default area where everyone goes";
            defaultArea.LongDescription = defaultArea.ShortDescription;

            Room room = new Room();
            room.URI = "DefaultRoom";
            room.Title = "The Default Room";
            room.ShortDescription = "This is the default room";
            room.LongDescription = "This is the default room.  It is very basic";

            defaultArea.Rooms[room.URI] = room;
            room.Area = defaultArea;
            globalLists.Areas[defaultArea.URI] = defaultArea;
        }

    }
}

using System;
using System.Collections.Generic;
using System.Text;
using Mirage.Core.Data.Attribute;
using Mirage.Core.IO.Serialization;
using Mirage.Core;
using Mirage.Core.Data;
using Mirage.Stock.Data.MobAI;

namespace Mirage.Stock.Data
{
    public class AreaLoader : IInitializer
    {
        private MudRepositoryBase _mudRespository;

        public AreaLoader(MudRepositoryBase MudRepository)
        {
            _mudRespository = MudRepository;
        }

        public string Name
        {
            get { return this.GetType().Name; }
        }

        public void Execute()
        {
            Area defaultArea = null;
            IPersistenceManager persister = ObjectStorageFactory.GetPersistenceManager(typeof(Area));
            defaultArea = (Area) persister.Load("DefaultArea");
            if (defaultArea == null)
            {

                defaultArea = new Area();
                defaultArea.Uri = "DefaultArea";
                defaultArea.Title = "Default Area";
                defaultArea.ShortDescription = "This is the default area where everyone goes";
                defaultArea.LongDescription = defaultArea.ShortDescription;

                Room room = new Room();
                room.Uri = "DefaultRoom";
                room.Title = "The Default Room";
                room.ShortDescription = "This is the default room";
                room.LongDescription = "This is the default room.  It is very basic";
                room.Exits[DirectionType.East] = new RoomExit(DirectionType.East, "SecondRoom", room);
                defaultArea.Rooms[room.Uri] = room;
                room.Area = defaultArea;

                room = new Room();
                room.Uri = "SecondRoom";
                room.Title = "The Second Room";
                room.ShortDescription = "This is the second room";
                room.LongDescription = "This is the second room.  It is a little more advanced than the default room, but still pretty basic";
                RoomExit westExit = new RoomExit(DirectionType.West, "/Areas/DefaultArea/Rooms/DefaultRoom", room);
                westExit.AddAttribute(new OpenableAttribute(westExit, false));
                room.Exits[DirectionType.West] = westExit;
                defaultArea.Rooms[room.Uri] = room;
                room.Area = defaultArea;
                persister.Save(defaultArea, defaultArea.Uri);
            }
            _mudRespository.Areas[defaultArea.Uri] = defaultArea;
            Mobile mob = new Mobile("first mobile");
            mob.ShortDescription = "the first mob";
            mob.LongDescription = "He is dressed in a new mob uniform waiting for orders.";
            mob.Level = 1;
            mob.Gender = GenderType.Male;
            mob.Programs.Add(new Wanderer(mob));
            mob.Programs.Add(new EchoProgram(mob));
            ((StockRepository)_mudRespository).Mobiles.Add(mob);
            defaultArea.Rooms["DefaultRoom"].Add(mob);
        }

    }
}
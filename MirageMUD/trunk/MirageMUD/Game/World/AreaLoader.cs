using System;
using System.Collections.Generic;
using System.Text;
using Mirage.Game.World.Attribute;
using Mirage.IO.Serialization;
using Mirage.Core;
using Mirage.Game.World;
using Mirage.Game.World.MobAI;
using Mirage.Game.World.Items;

namespace Mirage.Game.World
{
    public class AreaLoader : IInitializer
    {
        private IAreaRepository _areaRespository;
        private IMobileRepository _mobileRepository;

        public AreaLoader(IAreaRepository AreaRepository, IMobileRepository MobileRepository)
        {
            _areaRespository = AreaRepository;
            _mobileRepository = MobileRepository;
        }

        public string Name
        {
            get { return this.GetType().Name; }
        }

        public void Execute()
        {
            Area defaultArea = (Area) _areaRespository.Load("DefaultArea");
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
                _areaRespository.Save(defaultArea);                
            }
            _areaRespository.Update(defaultArea);

            Mobile mob = CreateMobile();
            _mobileRepository.Mobiles.Add(mob);
            defaultArea.Rooms["DefaultRoom"].Add(mob);

            defaultArea.Rooms["DefaultRoom"].Add(CreateItem());
            defaultArea.Rooms["DefaultRoom"].Add(CreateItem());
            defaultArea.Rooms["DefaultRoom"].Add(CreateItem());
            defaultArea.Rooms["DefaultRoom"].Add(CreateItem());
            defaultArea.Rooms["DefaultRoom"].Add(CreateItem());
            defaultArea.Rooms["DefaultRoom"].Add(CreateHelmet());
            Race.SaveRaces();
        }

        private Mobile CreateMobile()
        {
            Mobile mob = new Mobile(null);
            mob.Uri = "FirstMob";
            mob.ShortDescription = "the first mob";
            mob.LongDescription = "He is dressed in a new mob uniform waiting for orders.";
            mob.Title = "First Mob";
            mob.Level = 1;
            mob.Gender = GenderType.Male;
            mob.Programs.Add(new Wanderer(mob));
            mob.Programs.Add(new EchoProgram(mob));
            return mob;
        }

        private ItemBase CreateItem()
        {
            ItemBase item = new ItemBase();
            item.Uri = "DefaultItem";
            item.Title = "Default Item";
            item.ShortDescription = "a default item";
            item.LongDescription = "This is a very basic item.";
            return item;
        }

        private Armor CreateHelmet()
        {
            Armor item = new Armor();
            item.Uri = "Helmet";
            item.Title = "Helmet";
            item.ShortDescription = "a helmet";
            item.LongDescription = "This is a metal helmet";
            item.WearFlags = WearLocations.Head;
            return item;
        }
    }
}
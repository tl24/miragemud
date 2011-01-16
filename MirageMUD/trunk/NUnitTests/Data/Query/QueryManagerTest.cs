using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Mirage.Core.Data;
using NUnitTests.Mock;
using Mirage.Core.Data.Query;
using Mirage.Core.Data;

namespace NUnitTests.Data.Query
{
    [TestFixture]
    public class QueryManagerTest
    {
        [Test]
        public void QueryPlayerTest()
        {
            MockUriContainer bd = new MockUriContainer();
            LinkedList<Player> players = new LinkedList<Player>();
            bd.AddObject("Players", players, QueryHints.UniqueItems);
            Player abc = new Player();
            abc.Uri = "Abc";
            players.AddLast(abc);

            Player def = new Player();
            def.Uri = "Def";
            players.AddLast(def);

            Player defActual = (Player) new QueryManager().Find(bd, "Players/def");
            Assert.IsNotNull(defActual, "No player returned");
            Assert.AreSame(def, defActual, "Player not expected");
        }

        [Test]
        public void NoMatchTest()
        {
            MockUriContainer bd = new MockUriContainer();
            Player defActual = (Player)new QueryManager().Find(bd, "Players/def");
            Assert.IsNull(defActual, "No match should be returned");
        }

        [Test]
        public void PartialMatchTest()
        {
            MockUriContainer bd = new MockUriContainer();
            LinkedList<Player> players = new LinkedList<Player>();
            bd.AddObject("Players", players, QueryHints.UniqueItems);
            Player abc = new Player();
            abc.Uri = "Abc";
            players.AddLast(abc);

            Player def = new Player();
            def.Uri = "Def";
            players.AddLast(def);

            Player defActual = (Player)new QueryManager().Find(bd, "Players/d*");
            Assert.IsNotNull(defActual, "No player returned");
            Assert.AreSame(def, defActual, "Player not expected");
        }

        [Test]
        public void IndexedTest()
        {
            MockUriContainer bd = new MockUriContainer();
            LinkedList<Player> players = new LinkedList<Player>();
            bd.AddObject("Players", players, 0);
            Player abc = new Player();
            abc.Uri = "Abc";
            players.AddLast(abc);

            Player def = new Player();
            def.Uri = "Def";
            players.AddLast(def);

            Player dekken = new Player();
            dekken.Uri = "Dekken";
            players.AddLast(dekken);

            Player dekkenActual = (Player)new QueryManager().Find(bd, "Players/d*", 1);
            Assert.IsNotNull(dekkenActual, "No player returned");
            Assert.AreSame(dekken, dekkenActual, "Player not expected");
        }

        [Test]
        public void RoomQueryTest()
        {
            MockUriContainer bd = new MockUriContainer();
            IDictionary<string, Area> _areas = new Dictionary<string, Area>();
            bd.AddObject("Areas", _areas, QueryHints.UniqueItems | QueryHints.UriKeyedDictionary);
            Area area1 = new Area();
            area1.Uri = "Area1";
            _areas[area1.Uri] = area1;

            Room room1 = new Room();
            room1.Uri = "Room1";
            area1.Rooms.Add(room1.Uri, room1);

            Area area2 = new Area();
            area2.Uri = "Area2";
            _areas[area2.Uri] = area2;

            Room room2 = new Room();
            room2.Uri = "Room2";
            area2.Rooms.Add(room2.Uri, room2);

            Room room2Actual = (Room) new QueryManager().Find(bd, "Areas/Area2/Rooms/Room2");
            Assert.AreSame(room2, room2Actual, "RoomQuery returned wrong room");
     
        }

        [Test]
        public void UniqueItemIndexedTest()
        {
            MockUriContainer bd = new MockUriContainer();
            IDictionary<string, Area> _areas = new Dictionary<string, Area>();
            bd.AddObject("Areas", _areas, QueryHints.UniqueItems | QueryHints.UriKeyedDictionary);
            Area area1 = new Area();
            area1.Uri = "Area1";
            _areas[area1.Uri] = area1;

            Room room1 = new Room();
            room1.Uri = "Room1";
            area1.Rooms.Add(room1.Uri, room1);

            Area area2 = new Area();
            area2.Uri = "Area2";
            _areas[area2.Uri] = area2;

            Room room2 = new Room();
            room2.Uri = "Room2";
            area2.Rooms.Add(room2.Uri, room2);

            Room room2Actual = (Room)new QueryManager().Find(bd, "Areas/Area2/Rooms/Room2", 1);
            Assert.IsNull(room2Actual, "RoomQuery should have returned nothing");

        }

    }
}

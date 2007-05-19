using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Shoop.Data;
using NUnitTests.Mock;
using Shoop.Data.Query;

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
            bd.AddObject("Players", players, QueryCollectionFlags.UniqueItems);
            Player abc = new Player();
            abc.Uri = "Abc";
            players.AddLast(abc);

            Player def = new Player();
            def.Uri = "Def";
            players.AddLast(def);

            Player defActual = (Player) QueryManager.GetInstance().Find(bd, "Players/def");
            Assert.IsNotNull(defActual, "No player returned");
            Assert.AreSame(def, defActual, "Player not expected");
        }

        [Test]
        public void NoMatchTest()
        {
            MockUriContainer bd = new MockUriContainer();
            Player defActual = (Player)QueryManager.GetInstance().Find(bd, "Players/def");
            Assert.IsNull(defActual, "No match should be returned");
        }
    }
}

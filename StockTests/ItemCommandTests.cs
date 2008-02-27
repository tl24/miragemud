using System;
using System.Collections.Generic;
using System.Text;
using MbUnit.Framework;
using Mirage.Stock.Command;
using Mirage.Core.Data.Query;
using Mirage.Stock.Data;
using Mirage.Stock.Data.Items;
using Mirage.Core.Communication;

namespace StockTests
{
    [TestFixture]
    public class ItemCommandTests
    {
        ItemCommands cmd;
        Player player;
        Area area;
        Room room;

        [SetUp]
        public void TestSetup()
        {
            cmd = new ItemCommands();
            QueryManager qmgr = new QueryManager();
            cmd.QueryManager = qmgr;
            cmd.MessageFactory = new MessageFactory();

            player = new Player();
            MockClient client = new MockClient();
            client.Player = player;
            player.Client = client;

            area = new Area();
            room = new Room();
            room.Area = area;
            room.Uri = "Room";
            area.Rooms["Room"] = room;

            player.Container = room;
        }

        [Test]
        public void GetItem_WhenItemExists_ItemIsTaken()
        {
            ItemBase item = new ItemBase();
            item.Uri = "Item";
            room.Items.Add(item);
            item.Container = room;
            IMessage m = cmd.get_item(player, "Item");

            Assert.IsTrue(player.Inventory.Contains(item), "Player should have item");
            Assert.IsFalse(room.Items.Contains(item), "Room should not contain item");
        }

        [Test]
        public void Drop_WhenItemIsHeld_ItemDropped()
        {
            ItemBase item = new ItemBase();
            item.Uri = "Item";
            player.Inventory.Add(item);
            item.Container = player;
            IMessage m = cmd.drop(player, "Item");

            Assert.IsFalse(player.Inventory.Contains(item), "Player should NOT have item");
            Assert.IsTrue(room.Items.Contains(item), "Room SHOULD contain item");
        }
    }
}

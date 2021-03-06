using Mirage.Game.Command;
using Mirage.Game.World;
using Mirage.Game.World.Items;
using Mirage.Game.World.Query;
using NUnit.Framework;

namespace NUnitTests
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
            player = new Player();
            MockClient client = new MockClient();
            client.ClientState.Player = player;
            player.Client = client;

            area = new Area();
            room = new Room();
            room.Area = area;
            room.Uri = "Room";
            area.Rooms["Room"] = room;

            player.Room = room;
        }

        [Test]
        public void GetItem_WhenItemExists_ItemIsTaken()
        {
            ItemBase item = new ItemBase();
            item.Uri = "Item";
            room.Items.Add(item);
            item.Container = room;
            cmd.get_item(player, "Item");

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
            cmd.drop(player, "Item");

            Assert.IsFalse(player.Inventory.Contains(item), "Player should NOT have item");
            Assert.IsTrue(room.Items.Contains(item), "Room SHOULD contain item");
        }
    }
}

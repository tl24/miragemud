using Mirage.Game.Communication;
using Mirage.Game.World;
using NUnit.Framework;
using Mirage.Core.Messaging;

namespace NUnitTests.Communication
{
    [TestFixture]
    public class MessageSendingExtTests
    {
        private Player GetPlayer(string title, GenderType gender)
        {
            Player p = new Player();
            p.Name = title;
            p.ShortDescription = "a " + title;
            p.Gender = gender;
            MockClient client = new MockClient();
            client.Player = p;
            p.Client = client;
            return p;
        }

        [Test]
        public void TestToSelfNoArgs()
        {
            Player actor = GetPlayer("foo", GenderType.Male);
            Player victim = GetPlayer("vic", GenderType.Female);

            actor.ToSelf("testself", "You hit ${target}!", victim);
            MockClient actClient = (MockClient) actor.Client;
            MockClient vicClient = (MockClient) victim.Client;
            Assert.AreEqual(1, actClient.Messages.Count, "actor message count");
            Assert.AreEqual(0, vicClient.Messages.Count, "victim shouldn't have messages");

            Assert.AreEqual("You hit vic!\r\n", actClient.Messages[0].Render(), "Actor message text");
            Assert.AreEqual("testself.self", actClient.Messages[0].Name.FullName, "Actor message messageName");
            Assert.AreEqual(MessageType.Information, actClient.Messages[0].MessageType, "actor message type");
        }

        [Test]
        public void TestToSelfWithArgs()
        {
            Player actor = GetPlayer("foo", GenderType.Male);
            Player victim = GetPlayer("vic", GenderType.Female);

            actor.ToSelf("testself", "You hit ${target} with ${item}!", victim, new { item = "a bat" });
            MockClient actClient = (MockClient)actor.Client;
            MockClient vicClient = (MockClient)victim.Client;
            Assert.AreEqual(1, actClient.Messages.Count, "actor message count");
            Assert.AreEqual(0, vicClient.Messages.Count, "victim shouldn't have messages");

            Assert.AreEqual("You hit vic with a bat!\r\n", actClient.Messages[0].Render(), "Actor message text");
            Assert.AreEqual("vic", actClient.Messages[0]["target"], "actor message title property");
            Assert.AreEqual("a bat", actClient.Messages[0]["item"], "actor message title property");
        }

        [Test]
        public void TestToTargetNoArgs()
        {
            Player actor = GetPlayer("foo", GenderType.Male);
            Player victim = GetPlayer("vic", GenderType.Female);

            actor.ToTarget("testtarget", "${actor} hits you!", victim);
            MockClient actClient = (MockClient)actor.Client;
            MockClient vicClient = (MockClient)victim.Client;
            Assert.AreEqual(0, actClient.Messages.Count, "actor shouldn't have messages");
            Assert.AreEqual(1, vicClient.Messages.Count, "target message count");

            Assert.AreEqual("Foo hits you!\r\n", vicClient.Messages[0].Render(), "Target message text");
            Assert.AreEqual("testtarget.target", vicClient.Messages[0].Name.FullName, "Target message messageName");
        }

        [Test]
        public void TestToTargetWithArgs()
        {
            Player actor = GetPlayer("foo", GenderType.Male);
            Player victim = GetPlayer("vic", GenderType.Female);

            actor.ToTarget("testtarget", "${actor} hits you with ${item}!", victim, new { item = "a bat" });
            MockClient actClient = (MockClient)actor.Client;
            MockClient vicClient = (MockClient)victim.Client;
            Assert.AreEqual(0, actClient.Messages.Count, "actor shouldn't have messages");
            Assert.AreEqual(1, vicClient.Messages.Count, "target message count");

            Assert.AreEqual("Foo hits you with a bat!\r\n", vicClient.Messages[0].Render(), "Target message text");
        }

        [Test]
        public void TestToBystandersNoArgs()
        {
            Player actor = GetPlayer("foo", GenderType.Male);
            Player victim = GetPlayer("vic", GenderType.Female);
            Player bystander = GetPlayer("bystander", GenderType.Female);

            Room room = new Room();
            room.Add(actor);
            room.Add(victim);
            room.Add(bystander);
            actor.ToBystanders("testbystander", "${actor} hits ${target}!", victim);
            MockClient actClient = (MockClient)actor.Client;
            MockClient vicClient = (MockClient)victim.Client;
            MockClient bsClient = (MockClient)bystander.Client;
            Assert.AreEqual(0, actClient.Messages.Count, "actor shouldn't have messages");
            Assert.AreEqual(0, vicClient.Messages.Count, "target shouldn't have messages");
            Assert.AreEqual(1, bsClient.Messages.Count, "target message count");

            Assert.AreEqual("Foo hits vic!\r\n", bsClient.Messages[0].Render(), "Target message text");
            Assert.AreEqual("testbystander.others", bsClient.Messages[0].Name.FullName, "Target message messageName");
        }

        [Test]
        public void TestToBystandersWithArgs()
        {
            Player actor = GetPlayer("foo", GenderType.Male);
            Player victim = GetPlayer("vic", GenderType.Female);
            Player bystander = GetPlayer("bystander", GenderType.Female);

            Room room = new Room();
            room.Add(actor);
            room.Add(victim);
            room.Add(bystander);
            actor.ToBystanders("testbystander", "${actor} hits ${target} with ${item}!", victim, new { item = "a bat" });
            MockClient actClient = (MockClient)actor.Client;
            MockClient vicClient = (MockClient)victim.Client;
            MockClient bsClient = (MockClient)bystander.Client;
            Assert.AreEqual(0, actClient.Messages.Count, "actor shouldn't have messages");
            Assert.AreEqual(0, vicClient.Messages.Count, "target shouldn't have messages");
            Assert.AreEqual(1, bsClient.Messages.Count, "bystander message count");

            Assert.AreEqual("Foo hits vic with a bat!\r\n", bsClient.Messages[0].Render(), "Target message text");
            Assert.AreEqual("testbystander.others", bsClient.Messages[0].Name.FullName, "Target message messageName");
        }

        [Test]
        public void TestToRoomNoArgs()
        {
            Player actor = GetPlayer("foo", GenderType.Male);
            Player victim = GetPlayer("vic", GenderType.Female);
            Player bystander = GetPlayer("bystander", GenderType.Female);

            Room room = new Room();
            room.Add(actor);
            room.Add(victim);
            room.Add(bystander);
            actor.ToRoom("testroom", "${actor} hits ${target}!", victim);
            MockClient actClient = (MockClient)actor.Client;
            MockClient vicClient = (MockClient)victim.Client;
            MockClient bsClient = (MockClient)bystander.Client;
            Assert.AreEqual(0, actClient.Messages.Count, "actor shouldn't have messages");
            Assert.AreEqual(1, vicClient.Messages.Count, "target message count");
            Assert.AreEqual(1, bsClient.Messages.Count, "bystander message count");

            Assert.AreEqual("Foo hits vic!\r\n", vicClient.Messages[0].Render(), "Target message text");
            Assert.AreEqual("Foo hits vic!\r\n", bsClient.Messages[0].Render(), "Bystander message text");
            Assert.AreEqual("testroom.target", vicClient.Messages[0].Name.FullName, "Target message messageName");
            Assert.AreEqual("testroom.others", bsClient.Messages[0].Name.FullName, "Bystander message messageName");
        }

        [Test]
        public void TestToRoomWithArgs()
        {
            Player actor = GetPlayer("foo", GenderType.Male);
            Player victim = GetPlayer("vic", GenderType.Female);
            Player bystander = GetPlayer("bystander", GenderType.Female);

            Room room = new Room();
            room.Add(actor);
            room.Add(victim);
            room.Add(bystander);
            actor.ToRoom("testroom", "${actor} hits ${target} with ${item}!", victim, new { item = "a bat" });
            MockClient actClient = (MockClient)actor.Client;
            MockClient vicClient = (MockClient)victim.Client;
            MockClient bsClient = (MockClient)bystander.Client;
            Assert.AreEqual(0, actClient.Messages.Count, "actor shouldn't have messages");
            Assert.AreEqual(1, vicClient.Messages.Count, "target message count");
            Assert.AreEqual(1, bsClient.Messages.Count, "bystander message count");

            Assert.AreEqual("Foo hits vic with a bat!\r\n", vicClient.Messages[0].Render(), "Target message text");
            Assert.AreEqual("Foo hits vic with a bat!\r\n", bsClient.Messages[0].Render(), "Bystander message text");
            Assert.AreEqual("testroom.target", vicClient.Messages[0].Name.FullName, "Target message messageName");
            Assert.AreEqual("testroom.others", bsClient.Messages[0].Name.FullName, "Bystander message messageName");
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using Mirage.Core.Data;
using Mirage.Core.Communication;
using NUnit.Framework;
using Mirage.Core.Data.Skills;
using Mirage.Core.Data.Items;

namespace NUnitTests.Communication
{
    [TestFixture]
    public class MessageFormatterTest
    {
        private Player GetPlayer(string title, GenderType gender)
        {
            Player p = new Player();
            p.Title = title;
            p.ShortDescription = "a " + title;
            p.Gender = gender;
            return p;
        }

        private Mobile GetMobile(string title, GenderType gender)
        {
            MobTemplate template = new MobTemplate();
            Mobile m = new Mobile(template);
            m.Title = title;
            m.ShortDescription = "a " + title;
            m.Gender = gender;
            return m;

        }
        private MessageFormatter formatter = new MessageFormatter();

        [Test]
        public void TestFormat_PlayerActorName()
        {
            Player chr = GetPlayer("foo", GenderType.Male);
            StringMessage msg = formatter.Format(chr, chr, "test", "Hi, ${actor.name}!");
            Assert.AreEqual("Hi, foo!\r\n", GetMessageText(msg));
        }

        [Test]
        public void TestFormat_MobileActorName()
        {
            Mobile chr = GetMobile("foomob", GenderType.Male);
            StringMessage msg = formatter.Format(chr, chr, "test", "Hi, ${actor.name}!");
            Assert.AreEqual("Hi, foomob!\r\n", GetMessageText(msg));
        }

        [Test]
        public void TestFormat_PlayerTargetName()
        {
            Player chr = GetPlayer("foo", GenderType.Male);
            Player target = GetPlayer("bar", GenderType.Male);
            StringMessage msg = formatter.Format(chr, chr, "test", "Hi, ${actor.name} says ${target.name}!", target);
            Assert.AreEqual("Hi, foo says bar!\r\n", GetMessageText(msg));
        }

        [Test]
        public void TestFormat_MobileTargetName()
        {
            Mobile chr = GetMobile("foomob", GenderType.Male);
            Mobile target = GetMobile("barmob", GenderType.Male);
            StringMessage msg = formatter.Format(chr, chr, "test", "Hi, ${actor.name} says ${target.name}!", target);
            Assert.AreEqual("Hi, foomob says barmob!\r\n", GetMessageText(msg));
        }

        [TestCase(GenderType.Male, "He")]
        [TestCase(GenderType.Female, "She")]
        [TestCase(GenderType.Other, "It")]
        public void TestFormat_He(GenderType gender, string expected)
        {
            Player chr = GetPlayer("foo", gender);
            StringMessage msg = formatter.Format(chr, chr, "test", "${actor.he} jumps!");
            Assert.AreEqual(expected + " jumps!\r\n", GetMessageText(msg));
        }

        [TestCase(GenderType.Male, "He")]
        [TestCase(GenderType.Female, "She")]
        [TestCase(GenderType.Other, "It")]
        public void TestFormat_She(GenderType gender, string expected)
        {
            Player chr = GetPlayer("foo", gender);
            StringMessage msg = formatter.Format(chr, chr, "test", "${actor.she} jumps!");
            Assert.AreEqual(expected + " jumps!\r\n", GetMessageText(msg));
        }

        [TestCase(GenderType.Male, "Him\r\n")]
        [TestCase(GenderType.Female, "Her\r\n")]
        [TestCase(GenderType.Other, "It\r\n")]
        public void TestFormat_Him(GenderType gender, string expected)
        {
            Player chr = GetPlayer("foo", gender);
            StringMessage msg = formatter.Format(chr, chr, "test", "${actor.him}");
            Assert.AreEqual(expected, GetMessageText(msg));
        }

        [TestCase(GenderType.Male, "His\r\n")]
        [TestCase(GenderType.Female, "Her\r\n")]
        [TestCase(GenderType.Other, "Its\r\n")]
        public void TestFormat_His(GenderType gender, string expected)
        {
            Player chr = GetPlayer("foo", gender);
            StringMessage msg = formatter.Format(chr, chr, "test", "${actor.his}");
            Assert.AreEqual(expected, GetMessageText(msg));
        }

        [Test]
        public void TestFormat_Short()
        {
            Player chr = GetPlayer("foo", GenderType.Male);
            StringMessage msg = formatter.Format(chr, chr, "test", "${actor.short} is here.");
            Assert.AreEqual("A foo is here.\r\n", GetMessageText(msg));
        }

        [Test]
        public void TestFormat_WithObject()
        {
            Player chr = GetPlayer("foo", GenderType.Male);
            Armor helmet = new Armor();
            helmet.Title = "Gold Helmet";
            helmet.ShortDescription = "a gold helmet";
            StringMessage msg = formatter.Format(chr, chr, "test", "${object.short} is here.", null, new Dictionary<string, object> { { "object", helmet }});
            Assert.AreEqual("A gold helmet is here.\r\n", GetMessageText(msg));
        }

        [Test]
        public void TestFormat_WithObject0()
        {
            Player chr = GetPlayer("foo", GenderType.Male);
            Armor helmet = new Armor();
            helmet.Title = "Gold Helmet";
            helmet.ShortDescription = "a gold helmet";
            StringMessage msg = formatter.Format(chr, chr, "test", "${object.short} is here.", null, new Dictionary<string, object> { { "object0", helmet } });
            Assert.AreEqual("A gold helmet is here.\r\n", GetMessageText(msg));
        }

        [Test]
        public void TestFormat_FullWithMultipleObjects()
        {
            Player recipient = GetPlayer("foo", GenderType.Male);
            Player actor = GetPlayer("boo", GenderType.Male);
            Player target = GetPlayer("goo", GenderType.Female);
            Armor helmet = new Armor();
            helmet.Title = "Gold Helmet";
            helmet.ShortDescription = "a gold helmet";
            ItemBase bag = new ItemBase();
            bag.Title = "bag";
            bag.ShortDescription = "a bag";
            var args = new Dictionary<string, object> { { "object0", helmet }, { "object1", bag } };
            StringMessage msg = formatter.Format(recipient, actor, "test", "${actor.title} sees ${target.title} put ${target.his} ${object} in ${object1.short}.", target, args);
            Assert.AreEqual("Boo sees goo put her Gold Helmet in a bag.\r\n", GetMessageText(msg));
        }

        [Test]
        public void TestFormat_ErrorNamespace()
        {
            Player actor = GetPlayer("boo", GenderType.Male);
            StringMessage msg = formatter.Format(actor, actor, "error.test", "An error happened");
            Assert.AreEqual(MessageType.PlayerError, msg.MessageType);
        }

        [Test]
        public void TestFormat_NonError()
        {
            Player actor = GetPlayer("boo", GenderType.Male);
            StringMessage msg = formatter.Format(actor, actor, "test", "Info");
            Assert.AreEqual(MessageType.Information, msg.MessageType);
        }

        private string GetMessageText(StringMessage message)
        {
            Assert.IsNotNull(message, "Message not formatted properly or invalid arguments, message is null");
            return message.Text;
        }
    }
}

using NUnit.Framework;
using Mirage.Core.Messaging;

namespace NUnitTests.Communication
{
    [TestFixture]
    public class MessageTest
    {

        [Test]
        public void TestIsMatchType()
        {
            Message target = new Message(MessageType.Prompt, "test");
            Assert.IsTrue(target.IsMatch(MessageType.Prompt), "IsMatch(MessageType) failed");
            Assert.IsFalse(target.IsMatch(MessageType.Confirmation), "IsMatch(MessageType) failed");
        }

        [Test]
        public void TestIsMatchNamespace()
        {
            Message target = new Message(MessageType.Information, "negotiation.authentication.test");
            Assert.IsTrue(target.IsMatch("negotiation.authentication"));
            Assert.IsTrue(target.IsMatch("negotiation"));
            Assert.IsFalse(target.IsMatch("common.error"));
        }

        [Test]
        public void TestIsMatchName()
        {
            Message target = new Message(MessageType.Information, "negotiation.authentication.test");
            Assert.IsTrue(target.IsMatch("negotiation.authentication", "test"));
            Assert.IsTrue(target.IsMatch("negotiation.authentication" + ".test"));
            Assert.IsFalse(target.IsMatch("negotiation", "test"));
        }
    }
}

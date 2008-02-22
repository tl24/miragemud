using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Mirage.Core.Communication;

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
            Message target = new Message(MessageType.Information, Namespaces.Authentication, "test");
            Assert.IsTrue(target.IsMatch(Namespaces.Authentication));
            Assert.IsTrue(target.IsMatch(Namespaces.Negotiation));
            Assert.IsFalse(target.IsMatch(Namespaces.CommonError));
        }

        [Test]
        public void TestIsMatchName()
        {
            Message target = new Message(MessageType.Information, Namespaces.Authentication, "test");
            Assert.IsTrue(target.IsMatch(Namespaces.Authentication, "test"));
            Assert.IsTrue(target.IsMatch(Namespaces.Authentication + ".test"));
            Assert.IsFalse(target.IsMatch(Namespaces.Negotiation, "test"));
        }
    }
}

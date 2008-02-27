using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Mirage.Core.Communication;

namespace NUnitTests.Communication
{
    [TestFixture]
    public class MessageNameTests
    {

        [Test]
        public void Equals_WhenNamesAreSame_EqualsReturnsTrue()
        {
            MessageName name1 = new MessageName("system.Message");
            MessageName name2 = new MessageName("system.message");
            Assert.AreEqual(name1, name2, "Messages should be equal");
        }

        [Test]
        public void Equals_WhenNamesAreDifferent_EqualsReturnsFalse()
        {
            MessageName name1 = new MessageName("system.Message");
            MessageName name2 = new MessageName("system.other");
            Assert.AreNotEqual(name1, name2, "Messages should NOT be equal");
        }

        [Test]
        public void Equals_WhenObjectIsNotMessageName_EqualsReturnsFalse()
        {
            MessageName name1 = new MessageName("system.Message");
            string name2 = "system.other";
            Assert.AreNotEqual(name1, name2, "MessageName can not Equal other type");
        }

        [Test]
        public void Name_WhenFullNameCtor_NameReturnsNameOnly()
        {
            MessageName name = new MessageName("system.Message");
            Assert.AreEqual("Message", name.Name, "Name portion of MessageName not returned");
        }

        [Test]
        public void Name_WhenNamespaceNameCtor_NameReturnsNameOnly()
        {
            MessageName name = new MessageName("system", "Message");
            Assert.AreEqual("Message", name.Name, "Name portion of MessageName not returned");
        }

        [Test]
        public void Namespace_WhenFullNameCtor_NamespaceReturnsNamespaceOnly()
        {
            MessageName name = new MessageName("system.Message");
            Assert.AreEqual("system", name.Namespace, "Namespace portion of MessageName not returned");
        }

        [Test]
        public void Namespace_WhenNamespaceNameCtor_NamespaceReturnsNamespaceOnly()
        {
            MessageName name = new MessageName("system", "Message");
            Assert.AreEqual("system", name.Namespace, "Namespace portion of MessageName not returned");
        }

        [Test]
        public void FullName_FullNameIsCorrect()
        {
            MessageName name = new MessageName("system.Message");
            Assert.AreEqual("system.Message", name.FullName, "FullName not correct");
        }
    }
}

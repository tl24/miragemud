using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Mirage.Game.World.Query;

namespace NUnitTests.Game.World.Query
{
    [TestFixture]
    public class ObjectUriTests
    {
        [TestCase("/")]
        [TestCase("/Players/foo")]
        [TestCase("/Players/foo/Items/bar/")]
        [TestCase("Relative")]
        [TestCase("Parent/Child")]
        public void TestValidUris(string uri)
        {
            var uriObj = new ObjectUri(uri);
        }

        [TestCase("")]
        [TestCase("  ")]
        [TestCase(null)]
        public void TestUriCannotBeEmpty(string uri)
        {
            Assert.Throws<ArgumentException>(() => new ObjectUri(uri));
        }

        [TestCase("//Players/foo")]
        [TestCase("/Players//foo//Items/bar/")]
        [TestCase("//")]
        public void TestInvalidUris(string uri)
        {
            Assert.Throws<FormatException>(() => new ObjectUri(uri));
        }

        [Test]
        public void TestIterateRelativeUri()
        {
            var parts = new ObjectUri("a/b/c").ToArray();
            CollectionAssert.AreEqual(new[] { "a", "b", "c" }, parts);
        }

        [Test]
        public void TestIterateAbsoluteUri()
        {
            var parts = new ObjectUri("/a/b/c").ToArray();
            CollectionAssert.AreEqual(new[] { "/", "a", "b", "c" }, parts);
        }

        [Test]
        public void TestUriPropertyIsNormalized()
        {
            var objUri = new ObjectUri("A/B/C/  ");
            Assert.AreEqual("A/B/C", objUri.Uri);
        }

        [TestCase("/", "a", "/a")]
        [TestCase("a", "b", "a/b")]
        [TestCase("a/b/c/", "/d/e/", "a/b/c/d/e")]
        [TestCase("/root", "child", "/root/child")]
        public void TestAppendValid(string root, string child, string result)
        {
            var rootUri = new ObjectUri(root);
            var childUri = rootUri.Append(child);
            Assert.AreEqual(result, childUri.Uri);
        }

        [TestCase("/", "//Players/foo")]
        [TestCase("a", "/Players//foo//Items/bar/")]
        [TestCase("a/b/c/", "//")]
        public void TestAppendInvalid(string root, string child)
        {
            var rootUri = new ObjectUri(root);
            Assert.Throws<FormatException>(() => rootUri.Append(child));
        }

        [TestCase("/", "")]
        [TestCase("a", "  ")]
        [TestCase("a/b/c/", null)]
        [TestCase("a", "/")]
        public void TestCannotAppendEmptyOrRootUri(string root, string child)
        {
            var rootUri = new ObjectUri(root);
            Assert.Throws<ArgumentException>(() => rootUri.Append(child));
        }
    }
}


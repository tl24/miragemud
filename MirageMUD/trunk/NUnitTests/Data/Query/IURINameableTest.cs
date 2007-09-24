using System;
using System.Text;
using System.Collections.Generic;
using Mirage.Core.Data;
using Mirage.Core.Data.Query;
using NUnit.Framework;

namespace NUnitTests.Data.Query
{
    /// <summary>
    ///This is a test class for rom.Data.ObjectQuery and is intended
    ///to contain all rom.Data.ObjectQuery Unit Tests
    ///</summary>
    [TestFixture]
    public class URIQueryTest
    {


        [Test]
        public void ParseURIOnlyTest()
        {
            string uri = "myTestURI";
            ObjectQuery query = ObjectQuery.parse(uri);

            Assert.AreEqual(uri, query.UriName, "Uri and queryURI are not equal");
            Assert.IsTrue(query.TypeName == null || query.TypeName == string.Empty, "TypeName parameter is not empty");
        }

        /// <summary>
        ///A test for parse (string)
        ///</summary>
        [Test]
        public void SimpleParseTest()
        {
            string typeName = "players";
            string uriName = "Ted";
            string uriQueryString = typeName + ":" + uriName;

            ObjectQuery actual;

            actual = ObjectQuery.parse(uriQueryString);

            ObjectQuery expected = new ObjectQuery(typeName, uriName);

            Assert.AreEqual(expected, actual, "rom.Data.ObjectQuery.parse did not return the expected value.");
        }

        [Test]
        public void ComplexParseTest()
        {
            string typeName = "players";
            string uriName = "Ted";
            string typeName2 = "objects";
            string uriName2 = "sword";
            string uriQueryString = typeName + ":" + uriName + "/" + typeName2 + ":" + uriName2;

            ObjectQuery actual;

            actual = ObjectQuery.parse(uriQueryString);

            ObjectQuery expected = new ObjectQuery(typeName, uriName, new ObjectQuery(typeName2, uriName2));

            Assert.AreEqual(expected, actual, "rom.Data.ObjectQuery.parse did not return the expected value.");
        }

        /// <summary>
        ///A test for ObjectQuery (string)
        ///</summary>
        [Test]
        public void ConstructorURITest()
        {
            string uri = "myURIName";

            ObjectQuery target = new ObjectQuery(uri);

            Assert.AreEqual(uri, target.UriName, "Uri's do not match on ObjectQuery ctor");
            Assert.IsNull(target.Subquery, "SubQuery should be null on ObjectQuery ctor");
            Assert.IsNull(target.TypeName, "TypeName should be null on ObjectQuery ctor");
        }

        /// <summary>
        ///A test for ObjectQuery (string, string)
        ///</summary>
        [Test]
        public void ConstructorTypeURITest()
        {
            string type = "objType";
            string uri = "myURIName";

            ObjectQuery target = new ObjectQuery(type, uri);

            Assert.AreEqual(type, target.TypeName, "TypeName's do not match on ObjectQuery ctor");
            Assert.AreEqual(uri, target.UriName, "Uri's do not match on ObjectQuery ctor");
            Assert.IsNull(target.Subquery, "SubQuery should be null on ObjectQuery ctor");
        }

        /// <summary>
        ///A test for ObjectQuery (string, string, ObjectQuery)
        ///</summary>
        [Test]
        public void ConstructorAllTest()
        {
            string type = "objType";
            string uri = "myURIName";
            ObjectQuery subQuery = new ObjectQuery("type2", "uri2");

            ObjectQuery target = new ObjectQuery(type, uri, subQuery);

            Assert.AreEqual(type, target.TypeName, "TypeName's do not match on ObjectQuery ctor");
            Assert.AreEqual(uri, target.UriName, "Uri's do not match on ObjectQuery ctor");
            Assert.AreSame(subQuery, target.Subquery, "SubQuery does not match on ObjectQuery ctor");
        }


        /// <summary>
        ///A test for Equals (object)
        ///</summary>
        [Test]
        public void EqualsTest()
        {
            string type = "players";
            string uri = "Ted";

            string type2 = "objects";
            string uri2 = "sword";

            ObjectQuery expected = new ObjectQuery(type, uri, new ObjectQuery(type2, uri2));

            ObjectQuery actual = new ObjectQuery(type, uri, new ObjectQuery(type2, uri2));

            Assert.AreEqual(expected, actual, "rom.Data.ObjectQuery.Equals did not return the expected value.");
        }

        [Test]
        public void ParseWithSubQueryTest()
        {
            ObjectQuery q = ObjectQuery.parse("/Players", "SomePlayer/Prop");
            Assert.AreEqual("Players", q.UriName);
            Assert.AreEqual("SomePlayer/Prop", q.Subquery.UriName);
        }
    }


}

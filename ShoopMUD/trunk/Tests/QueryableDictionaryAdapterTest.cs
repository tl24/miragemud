﻿// The following code was generated by Microsoft Visual Studio 2005.
// The test owner should check each test for validity.
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Text;
using System.Collections.Generic;
using Shoop.Data.Query;
using Shoop.Data;
namespace ROMtests
{
    /// <summary>
    ///This is a test class for Shoop.Data.Query.QueryableDictionaryAdapter&lt;V&gt; and is intended
    ///to contain all Shoop.Data.Query.QueryableDictionaryAdapter&lt;V&gt; Unit Tests
    ///</summary>
    [TestClass()]
    public class QueryableDictionaryAdapterTest
    {


        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }
        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        /// <summary>
        ///A test for Find (ObjectQuery)
        ///</summary>
        [TestMethod()]
        public void findTest()
        {
            string uri = null; // TODO: Initialize to an appropriate value
             
            IDictionary<string, Room> rooms = new SortedDictionary<string, Room>();
            Room r1 = new Room();
            r1.URI = "cafe";
            Room r2 = new Room();
            r2.URI = "cantina";

            rooms[r1.URI] = r1;
            rooms[r2.URI] = r2;
            QueryableDictionaryAdapter<Room> target = new QueryableDictionaryAdapter<Room>("rooms", rooms, QueryCollectionFlags.Sorted);

            ObjectQuery q1 = ObjectQuery.parse("cafe");
             
            IQueryable expected = r1;
            IQueryable actual;
             
            actual = target.Find(q1);
             
            Assert.AreEqual(expected, actual, "Shoop.Data.Query.QueryableDictionaryAdapter<Room>.Find [cafe] did not return the expected value");

            q1 = ObjectQuery.parse("cantina");

            expected = r2;
            actual = target.Find(q1);

            Assert.AreEqual(expected, actual, "Shoop.Data.Query.QueryableDictionaryAdapter<Room>.Find [cantina] did not return the expected value");

        }

        /// <summary>
        ///A test for QueryableDictionaryAdapter (string, IDictionary&lt;string,V&gt;)
        ///</summary>
        [TestMethod()]
        public void ConstructorTest()
        {
            // string uri = null; // TODO: Initialize to an appropriate value
            // 
            // System.Collections.Generic.IDictionary<string, V> collection = null; // TODO: Initialize to an appropriate value
            // 
            // QueryableDictionaryAdapter<V> target = new QueryableDictionaryAdapter<V>(uri, collection);
            // 
            // // TODO: Implement code to verify target
            // Assert.Inconclusive("TODO: Implement code to verify target");
            //Assert.Inconclusive("Generics testing must be manually provided.");
        }

        /// <summary>
        ///A test for QueryableDictionaryAdapter (string, IDictionary&lt;string,V&gt;, QueryCollectionFlags)
        ///</summary>
        [TestMethod()]
        public void ConstructorTest1()
        {
            // string uri = null; // TODO: Initialize to an appropriate value
            // 
            // System.Collections.Generic.IDictionary<string, V> collection = null; // TODO: Initialize to an appropriate value
            // 
            // QueryCollectionFlags flags = QueryCollectionFlags.DuplicatesAllowed; // TODO: Initialize to an appropriate value
            // 
            // QueryableDictionaryAdapter<V> target = new QueryableDictionaryAdapter<V>(uri, collection, flags);
            // 
            // // TODO: Implement code to verify target
            // Assert.Inconclusive("TODO: Implement code to verify target");
            //Assert.Inconclusive("Generics testing must be manually provided.");
        }

    }


}

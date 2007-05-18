﻿// The following code was generated by Microsoft Visual Studio 2005.
// The test owner should check each test for validity.
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Text;
using System.Collections.Generic;
using Shoop.Communication;
using System.Collections;
namespace ROMtests
{
    /// <summary>
    ///This is a test class for Shoop.Communication.TemplateManager and is intended
    ///to contain all Shoop.Communication.TemplateManager Unit Tests
    ///</summary>
    [TestClass()]
    public class TemplateManagerTest
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
        ///A test for CreateTemplate (string, string)
        ///</summary>
        [TestMethod()]
        public void CreateTemplateTest()
        {
            string name = "custom"; // TODO: Initialize to an appropriate value

            string templateText = "${player} goes ${dir}."; // TODO: Initialize to an appropriate value

            string dir = "west";
            string expected = "Ted goes " + dir + ".\r\n";

            ITemplate template = TemplateManager.CreateTemplate(name, templateText);
            string actual = "";
            string[] expectedAvailParams = new string[] { "player", "dir" };
            string[] actualParams = new string[2];

            template.AvailableParameters.CopyTo(actualParams, 0);

            for (int i = 0; i < expectedAvailParams.Length; i++)
            {
                Assert.AreEqual(expectedAvailParams[i], actualParams[i], "Available parameters are not correct");
            }
            template["dir"] = dir;
            template["player"] = "Ted";
            actual = template.Render();

            Assert.AreEqual(expected, actual, "Template did not render correctly.");
        }

        [TestMethod()]
        public void GetTemplateTest()
        {
            string name = "Movement.Arrival"; // TODO: Initialize to an appropriate value

            string templateText = "${player} goes ${dir}."; // TODO: Initialize to an appropriate value

            string player = "Ted";
            string expected = "Ted has arrived.\r\n";

            ITemplate template = TemplateManager.GetTemplate(name);
            string actual = "";
            string[] expectedAvailParams = new string[] { "player"};
            string[] actualParams = new string[1];

            template.AvailableParameters.CopyTo(actualParams, 0);

            for (int i = 0; i < expectedAvailParams.Length; i++)
            {
                Assert.AreEqual(expectedAvailParams[i], actualParams[i], "Available parameters are not correct");
            }
            template["player"] = player;
            actual = template.Render();

            Assert.AreEqual(expected, actual, "Template did not render correctly.");
        }

    }


}

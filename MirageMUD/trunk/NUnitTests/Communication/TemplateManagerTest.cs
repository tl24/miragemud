using System;
using System.Text;
using System.Collections.Generic;
using Mirage.Communication;
using System.Collections;
using NUnit.Framework;

namespace NUnitTests.Communication
{
    /// <summary>
    ///This is a test class for Mirage.Communication.TemplateManager and is intended
    ///to contain all Mirage.Communication.TemplateManager Unit Tests
    ///</summary>
    [TestFixture]
    public class TemplateManagerTest
    {

        /// <summary>
        ///A test for CreateTemplate (string, string)
        ///</summary>
        [Test]
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

        [Test]
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

        [Test]
        public void ReferencedTemplateTest()
        {

            string IncludedTemplate = "IncludedTemplate"; //I'm inside ${otherTemplate}.
            string TestTemplateInclude = "TestTemplateInclude"; //This template contains ( @{IncludedTemplate} ) custom = ${custom}

            ITemplate testTemplate = TemplateManager.GetTemplate(TestTemplateInclude);

            // verify that the referenced templates parameters are returned
            Dictionary<string, bool> parms = new Dictionary<string, bool>();
            parms["otherTemplate"] = true;
            parms["custom"] = true;

            ICollection<string> availParams = testTemplate.AvailableParameters;
            Assert.AreEqual(parms.Count, availParams.Count, "Wrong number of available params");

            foreach (string parm in availParams)
            {
                Assert.IsTrue(parms.ContainsKey(parm), "Wrong available param");
            }

            testTemplate["otherTemplate"] = "TestTemplateInclude";
            testTemplate["custom"] = "123456";

            string expected = "This template contains ( I'm inside TestTemplateInclude. ) custom = 123456\r\n";
            string actual = testTemplate.Render();
            Assert.AreEqual(expected, actual, "Template did not render correctly");
        }

        [Test]
        public void SpecialCharactersTest()
        {
            string templateName = "msg:/namespace/test";
            string expected = "Namespace test message";
            Uri nmspc = new Uri("msg:/namespace/");
            Uri combined = new Uri(nmspc, "test");

            Uri testBase = Namespaces.Authentication;
            Uri testName = new Uri(testBase, "Challenge");

            ITemplate template = TemplateManager.GetTemplate(combined.ToString());
            string actual = template.Render();
            Assert.AreEqual(expected, actual);
        }
    }


}

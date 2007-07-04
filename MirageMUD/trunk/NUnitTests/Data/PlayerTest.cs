using System;
using System.Text;
using System.Collections.Generic;
using Shoop.Data;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using NUnit.Framework;

namespace NUnitTests.Data
{
    /// <summary>
    ///This is a test class for rom.Data.Player and is intended
    ///to contain all rom.Data.Player Unit Tests
    ///</summary>
    [TestFixture]
    public class PlayerTest
    {

        /// <summary>
        ///A test for ComparePassword (string)
        ///</summary>
        [Test]
        public void comparePasswordTest()
        {
            Player target = new Player();
            target.SetPassword("mypassword");
            string otherPassword = "mypassword";

            bool expected = true;
            bool actual;

            actual = target.ComparePassword(otherPassword);

            Assert.AreEqual(expected, actual, "rom.Data.Player.ComparePassword did not return the expected value.");

            target.SetPassword("someotherpassword");
            expected = false;
            actual = target.ComparePassword(otherPassword);

            Assert.AreEqual(expected, actual, "rom.Data.Player.ComparePassword did not return the expected value.");
        }

        /// <summary>
        ///A test for password
        ///</summary>
        [Test]
        public void passwordTest()
        {
            Player target = new Player();

            string val = "mypassword"; // TODO: Assign to an appropriate value for the property

            target.SetPassword(val);


            Assert.AreNotEqual(val, target.Password, "rom.Data.Player.password was not encrypted.");
        }

        [Test]
        public void loadSaveTest()
        {
            Player target = new Player();
            target.Uri = "targetName";
            target.Title = "targetName";
            target.LongDescription = "target Description line 1\r\ntargeDescriptionon line 2";
            target.SetPassword("targetPassword");
            target.Level = 23;
            target.Sex = SexType.Female;

            Player.Save(target);
            Player loaded = Player.Load(target.Uri);

            Assert.AreEqual(target.Title, loaded.Title, "rom.Data.Player.Title field not equal to loaded value");
        }
    }


}

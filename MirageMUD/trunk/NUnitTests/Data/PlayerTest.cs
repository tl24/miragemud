using System;
using System.Text;
using System.Collections.Generic;
using Mirage.Core.Data;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using NUnit.Framework;
using Mirage.Stock.Data;

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
    }


}

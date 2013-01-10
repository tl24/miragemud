using Mirage.Game.World;
using NUnit.Framework;

namespace NUnitTests.Game.World
{
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

            Assert.AreEqual(expected, actual, "Player.ComparePassword did not return the expected value.");

            target.SetPassword("someotherpassword");
            expected = false;
            actual = target.ComparePassword(otherPassword);

            Assert.AreEqual(expected, actual, "Player.ComparePassword did not return the expected value.");
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


            Assert.AreNotEqual(val, target.Password, "Player.password was not encrypted.");
        }
    }


}

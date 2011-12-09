using Mirage.Game.Command;
using NUnit.Framework;

namespace NUnitTests.Command
{
    [TestFixture]
    public class ArgumentParserTests
    {
        [Test]
        public void TestEmpty()
        {
            ArgumentParser parser = new ArgumentParser("");
            Assert.IsTrue(parser.IsEmpty());
            Assert.IsNull(parser.GetNextArgument());
            Assert.IsNull(parser.GetRest());
        }

        [Test]
        public void TestSingleArg()
        {
            ArgumentParser parser = new ArgumentParser("arg");
            Assert.AreEqual("arg", parser.GetNextArgument());
            Assert.IsNull(parser.GetNextArgument());
        }

        [Test]
        public void TestSpaceDelimitedArgs()
        {
            ArgumentParser parser = new ArgumentParser("arg1 arg2 arg3");
            Assert.AreEqual("arg1", parser.GetNextArgument());
            Assert.AreEqual("arg2", parser.GetNextArgument());
            Assert.AreEqual("arg3", parser.GetNextArgument());
            Assert.IsNull(parser.GetNextArgument());
        }

        [Test]
        public void TestSingleQuotedArgs()
        {
            ArgumentParser parser = new ArgumentParser("arg1 'space arg2' 'space arg3'");
            Assert.AreEqual("arg1", parser.GetNextArgument());
            Assert.AreEqual("space arg2", parser.GetNextArgument());
            Assert.AreEqual("space arg3", parser.GetNextArgument());
            Assert.IsNull(parser.GetNextArgument());
        }

        [Test]
        public void TestDoubleQuotedArgs()
        {
            ArgumentParser parser = new ArgumentParser("arg1 \"space arg2\" \"space arg3\"");
            Assert.AreEqual("arg1", parser.GetNextArgument());
            Assert.AreEqual("space arg2", parser.GetNextArgument());
            Assert.AreEqual("space arg3", parser.GetNextArgument());
            Assert.IsNull(parser.GetNextArgument());
        }

        [Test]
        public void TestSpecialCharacters()
        {
            ArgumentParser parser = new ArgumentParser("arg1 namespace/name code.1.2");
            Assert.AreEqual("arg1", parser.GetNextArgument());
            Assert.AreEqual("namespace/name", parser.GetNextArgument());
            Assert.AreEqual("code.1.2", parser.GetNextArgument());
            Assert.IsNull(parser.GetNextArgument());
        }

        [Test]
        public void TestRestArgs()
        {
            string first = "say";
            string rest = "Hey there, my name's O'Reilly, what's yours?";
            ArgumentParser parser = new ArgumentParser(string.Format("{0} {1}", first, rest));
            Assert.AreEqual(first, parser.GetNextArgument());
            Assert.AreEqual(rest, parser.GetRest());
        }

        [Test]
        public void TestEnumerator()
        {
            string parsed = "arg1 arg2 'space arg3'";
            int i = 0;
            ArgumentParser parser = new ArgumentParser(parsed);
            foreach (string arg in parser)
            {
                switch (i)
                {
                    case 0:
                        Assert.AreEqual("arg1", arg);
                        break;
                    case 1:
                        Assert.AreEqual("arg2", arg);
                        break;
                    case 2:
                        Assert.AreEqual("space arg3", arg);
                        break;
                    default:
                        Assert.Fail("Too many arguments returned from the enumerator");
                        break;
                }
                i++;
            }
            Assert.AreEqual(3, i, "Not enough arguments returned from the enumerator");
            Assert.AreEqual(parsed, parser.GetRest(), "Enumerator modified the instance");
        }
    }
}

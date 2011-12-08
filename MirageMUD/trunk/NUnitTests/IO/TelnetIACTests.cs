using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using NUnitTests.Mock;
using Mirage.IO.Net;
using Castle.Core.Logging;
using Mirage.Core;
using Mirage.Telnet;

namespace NUnitTests.IO
{
    [TestFixture]
    public class TelnetIACTests
    {
        /*
        [Test]
        public void WhenDoEcho_WillEchoAndEchoOn()
        {
            MockTelnetClient client = new MockTelnetClient();
            client.Options.EchoOn = false;
            TelnetOptionProcessor proc = new TelnetOptionProcessor(client, NullLogger.Instance);
            proc.ProcessBuffer(new byte[] { (byte)TelnetCodes.IAC, (byte)TelnetCodes.DO, (byte) TelnetCodes.ECHO }, 3);
            Assert.IsTrue(client.Options.EchoOn, "EchoOn");
            byte[] result = client.OutputStream.ToArray();
            CollectionAssert.AreEqual(new byte[] { (byte)TelnetCodes.IAC, (byte)TelnetCodes.WILL, (byte)TelnetCodes.ECHO }, result, "Output Bytes");
        }

        [Test]
        public void WhenDontEcho_WontEchoAndEchoOff()
        {
            MockTelnetClient client = new MockTelnetClient();
            client.Options.EchoOn = true;
            TelnetOptionProcessor proc = new TelnetOptionProcessor(client, NullLogger.Instance);
            proc.ProcessBuffer(new byte[] { (byte)TelnetCodes.IAC, (byte)TelnetCodes.DONT, (byte)TelnetCodes.ECHO }, 3);
            Assert.IsFalse(client.Options.EchoOn, "EchoOff");
            byte[] result = client.OutputStream.ToArray();
            CollectionAssert.AreEqual(new byte[] { (byte)TelnetCodes.IAC, (byte)TelnetCodes.WONT, (byte)TelnetCodes.ECHO }, result, "Output Bytes");
        }

        [Test]
        public void WhenWillNaws_DoNawsAndReceiveWindowSize()
        {
            MockTelnetClient client = new MockTelnetClient();
            TelnetOptionProcessor proc = new TelnetOptionProcessor(client, NullLogger.Instance);
            proc.ProcessBuffer(new byte[] { (byte)TelnetCodes.IAC, (byte)TelnetCodes.WILL, (byte)TelnetCodes.WINDOW_SIZE }, 3);
            byte[] result = client.OutputStream.ToArray();
            CollectionAssert.AreEqual(new byte[] { (byte)TelnetCodes.IAC, (byte)TelnetCodes.DO, (byte)TelnetCodes.WINDOW_SIZE }, result, "Output Bytes");
            proc.ProcessBuffer(new byte[] { (byte)TelnetCodes.IAC, (byte)TelnetCodes.SB, (byte)TelnetCodes.WINDOW_SIZE,
                                            0, 24, 0, 80, (byte) TelnetCodes.IAC, (byte) TelnetCodes.SE});
            Assert.AreEqual(24, client.Options.WindowWidth, "Width");
            Assert.AreEqual(80, client.Options.WindowHeight, "Height");

        }
         */
    }
}

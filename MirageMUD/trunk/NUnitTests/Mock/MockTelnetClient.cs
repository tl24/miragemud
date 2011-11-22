using System;
using System.Collections.Generic;
using System.Text;
using Mirage.Core.IO;
using System.IO;
using Mirage.Telnet;

namespace NUnitTests.Mock
{
    public class MockTelnetClient : ClientBase, ITextClient, IClient, IClientNaws
    {

        public MockTelnetClient()
            : base(null)
        {
            this.OutputStream = new MemoryStream();
            this.Options = new TextClientOptions();
        }

        public MemoryStream OutputStream { get; set; }

        public TextClientOptions Options { get; set; }

        public override void ReadInput()
        {
        }

        public override void ProcessInput()
        {
        }

        public override void FlushOutput()
        {
        }

        public override void Write(Mirage.Core.Communication.IMessage message)
        {
            Write(Encoding.ASCII.GetBytes(message.Render()));
        }

        public void Write(byte[] bytes)
        {
            OutputStream.Write(bytes, 0, bytes.Length);
        }

        int IClientNaws.WindowWidth
        {
            get
            {
                return this.Options.WindowWidth;
            }
            set
            {
                this.Options.WindowWidth = value;
            }
        }

        int IClientNaws.WindowHeight
        {
            get
            {
                return this.Options.WindowHeight;
            }
            set
            {
                this.Options.WindowHeight = value;
            }
        }

        bool IClient.EchoOn
        {
            get
            {
                return this.Options.EchoOn;
            }
            set
            {
                this.Options.EchoOn = value;
            }
        }

    }
}

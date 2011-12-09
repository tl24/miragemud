using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mirage.IO.Net.Telnet.Options
{
    public class NawsEventArgs : SubNegotiationEventArgs
    {
        public NawsEventArgs(int height, int width)
            : base(OptionCodes.NAWS)
        {
            this.Height = height;
            this.Width = width;
        }

        /// <summary>
        /// The height of the client window
        /// </summary>
        public int Height { get; protected set; }

        /// <summary>
        /// The width of the client window
        /// </summary>
        public int Width { get; protected set; }
    }
    /// <summary>
    /// Option processor for TELNET NAWS which sends the window size
    /// </summary>
    public class NawsOption : TelnetOption
    {
        public NawsOption(TelnetOptionProcessor parent)
            : base(parent, OptionCodes.NAWS)
        {
        }

        public override void OnSubNegotiation(byte[] subData)
        {
            if (subData.Length == 4)
            {
                // data should be transmitted in big endian
                // if our system is little endian we need to convert before parsing
                if (BitConverter.IsLittleEndian)
                {
                    Array.Reverse(subData, 0, 2);
                    Array.Reverse(subData, 2, 2);
                }
                //IClientNaws nawsClient = (IClientNaws)Parent.Client;
                short w = BitConverter.ToInt16(subData, 0);
                short h = BitConverter.ToInt16(subData, 2);
                Parent.OnSubNegotiationOccurred(new NawsEventArgs(h, w));
            }
        }

    }

}

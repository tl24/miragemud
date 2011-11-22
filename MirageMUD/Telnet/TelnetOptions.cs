using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mirage.Telnet
{
    internal class TelnetOption 
    {
        private int state = 0;

        public TelnetOption(TelnetOptionProcessor parent, TelnetCodes optionValue) : this(parent, (byte) optionValue)
        {
        }

        public TelnetOption(TelnetOptionProcessor parent, byte optionValue)
        {
            this.Parent = parent;
            this.OptionValue = optionValue;
        }

        public byte OptionValue { get; private set; }

        public TelnetOptionProcessor Parent { get; private set; }

        /// <summary>
        /// The state of the option on the local end (Us value)
        /// </summary>
        public QState LocalState
        {
            get
            {
                return (QState)(state >> 4);
            }
            set
            {
                state = ((int)value) << 4 | (state & 0x0F);

            }
        }
        /// <summary>
        /// The state of the option on the remote end (Him value)
        /// </summary>
        public QState RemoteState
        {
            get
            {
                return (QState)(state & 0x0F);
            }
            set
            {
                state = ((state << 4) & 0xF0) | (((int)value) & 0x0F);
            }
        }

        public virtual void OnOptionChanged(bool enabled, bool local)
        {
        }

        protected void SendResponse(TelnetCodes optionCode)
        {
            Parent.LogLine(OptionValue.ToString("d"));
            Parent.LogLine(string.Format("Sending IAC {0:g} {1:d}", optionCode, OptionValue));
            Parent.SendBytes(new byte[] { (byte)TelnetCodes.IAC, (byte)optionCode, OptionValue });
        }

        public virtual void OnSubNegotiation(byte[] subData)
        {
            Parent.LogLine(string.Format("Option {0} does not support sub negotiation.  Received Byte Data: {1}", OptionValue, subData));

        }
    }

    internal class EchoOption : TelnetOption
    {
        public EchoOption(TelnetOptionProcessor parent) : base(parent, TelnetCodes.ECHO)
        {
        }

        public override void OnOptionChanged(bool enabled, bool local)
        {
            if (local)
            {
                Parent.Client.EchoOn = enabled;
            }
        }
    }

    internal class NawsOption : TelnetOption
    {
        private bool _enabled;

        public NawsOption(TelnetOptionProcessor parent)
            : base(parent, TelnetCodes.WINDOW_SIZE)
        {
            _enabled = parent.Client is IClientNaws;
            if (_enabled)
            {
                parent.Logger.Debug("Naws processing enabled");
            }
            else
            {
                parent.Logger.Debug("Naws processing disabled, the client does not implement ITelnetClientNaws");
            }
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
                IClientNaws nawsClient = (IClientNaws)Parent.Client;
                nawsClient.WindowWidth = BitConverter.ToInt16(subData, 0);
                nawsClient.WindowHeight = BitConverter.ToInt16(subData, 2);
            }
        }
        
    }
}

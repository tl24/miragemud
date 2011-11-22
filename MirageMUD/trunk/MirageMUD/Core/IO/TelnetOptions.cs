using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MirageMUD.Core;

namespace Mirage.Core.IO
{
    public class TelnetOption 
    {
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

        public virtual void OnDo()
        {
            // for now, we always Wont
            SendResponse(TelnetCodes.WONT);
        }

        public virtual void OnDont()
        {
            // for now, we always Wont
            SendResponse(TelnetCodes.WONT);
        }

        public virtual void OnWill()
        {
            // by default, we always Dont
            SendResponse(TelnetCodes.DONT);
        }

        public virtual void OnWont()
        {
            // by default, we always Dont
            SendResponse(TelnetCodes.DONT);
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

    public class EchoOption : TelnetOption
    {
        public EchoOption(TelnetOptionProcessor parent) : base(parent, TelnetCodes.ECHO)
        {
        }

        public override void OnDo()
        {
            SendResponse(TelnetCodes.WILL);
            Parent.Client.Options.EchoOn = true;
            Parent.SetState<TelnetTextState>();
        }

        public override void OnDont()
        {
            SendResponse(TelnetCodes.WONT);
            Parent.Client.Options.EchoOn = false;
            Parent.SetState<TelnetTextState>();
        }
    }

    public class NawsOption : TelnetOption
    {
        public NawsOption(TelnetOptionProcessor parent)
            : base(parent, TelnetCodes.WINDOW_SIZE)
        {
        }

        public override void OnWill()
        {
            SendResponse(TelnetCodes.DO);
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
                Parent.Client.Options.WindowWidth = BitConverter.ToInt16(subData, 0);
                Parent.Client.Options.WindowHeight = BitConverter.ToInt16(subData, 2);
            }
        }
        
    }
}

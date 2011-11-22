using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MirageMUD.Core;

namespace Mirage.Core.IO
{
    public abstract class TelnetState
    {
        protected TelnetOptionProcessor Parent { get; private set; }

        public TelnetState(TelnetOptionProcessor parent)
        {
            this.Parent = parent;
        }

        public abstract void ProcessByte(byte data);
    }

    /// <summary>
    /// This is the default state where we process text
    /// </summary>
    public class TelnetTextState : TelnetState
    {
        public TelnetTextState(TelnetOptionProcessor parent) : base(parent)
        {
        }

        public override void ProcessByte(byte data)
        {
            if (data == (byte)TelnetCodes.IAC)
            {
                Parent.SetState<TelnetIACState>();
                Parent.AppendLog("IAC");
            }
            else
            {
                Parent.AddOutputByte(data);
            }
        }
    }

    public class TelnetIACState : TelnetState
    {
        public TelnetIACState(TelnetOptionProcessor parent)
            : base(parent)
        {
        }

        public override void ProcessByte(byte data)
        {
            switch ((TelnetCodes)data)
            {
                case TelnetCodes.IAC:
                    Parent.AddOutputByte(data);
                    Parent.LogLine(((TelnetCodes)data).ToString("g"));
                    Parent.SetState<TelnetTextState>();
                    break;
                case TelnetCodes.DO:
                case TelnetCodes.DONT:
                    Parent.AppendLog(((TelnetCodes)data).ToString("g"));
                    Parent.SetState<TelnetDoDontState>();
                    break;
                case TelnetCodes.WILL:
                case TelnetCodes.WONT:
                    Parent.AppendLog(((TelnetCodes)data).ToString("g"));
                    Parent.SetState<TelnetWillWontState>();
                    break;
                case TelnetCodes.SB:
                    Parent.AppendLog(((TelnetCodes)data).ToString("g"));
                    Parent.SetState<TelnetSubNegotiationState>();
                    break;
                default:
                    Parent.AppendLog("Unrecognized byte sequence " + data.ToString("d"));
                    Parent.SetState<TelnetUnknownSequenceState>();
                    break;
            }
        }
    }

    public class TelnetDoDontState : TelnetState
    {
        public TelnetDoDontState(TelnetOptionProcessor parent)
            : base(parent)
        {
        }
        public override void ProcessByte(byte data)
        {
            TelnetOption option = Parent.LookupOption(data);
            bool isDo = Parent.LookBehind(1) == (byte)TelnetCodes.DO;
            if (isDo)
                option.OnDo();
            else
                option.OnDont();
            Parent.SetState<TelnetTextState>();
        }
    }

    public class TelnetWillWontState : TelnetState
    {
        public TelnetWillWontState(TelnetOptionProcessor parent)
            : base(parent)
        {
        }

        public override void ProcessByte(byte data)
        {
            TelnetOption option = Parent.LookupOption(data);
            bool isWill = Parent.LookBehind(1) == (byte)TelnetCodes.WILL;
            if (isWill)
                option.OnWill();
            else
                option.OnWont();
            Parent.SetState<TelnetTextState>();
        }
    }

    public class TelnetSubNegotiationState : TelnetState
    {
        List<byte> buffer = new List<byte>();
        bool lastWasIAC = false;

        public TelnetSubNegotiationState(TelnetOptionProcessor parent)
            : base(parent)
        {
        }

        public override void ProcessByte(byte data)
        {
            Parent.LogLine(data.ToString("d"));
            switch (data)
            {
                case (byte) TelnetCodes.IAC:
                    if (lastWasIAC)
                    {
                        //escape sequence
                        buffer.Add(data);
                        lastWasIAC = false;
                    }
                    else
                    {
                        lastWasIAC = true;
                    }
                    break;
                case (byte) TelnetCodes.SE:
                    if (lastWasIAC)
                    {
                        // first byte is the option
                        byte optionValue = buffer[0];
                        byte[] subData = new byte[buffer.Count - 1];
                        buffer.CopyTo(1, subData, 0, buffer.Count - 1);
                        TelnetOption option = Parent.LookupOption(optionValue);
                        option.OnSubNegotiation(subData);
                        buffer.Clear();
                        lastWasIAC = false;
                        Parent.SetState<TelnetTextState>();
                    }
                    else
                    {
                        // regular data
                        buffer.Add(data);
                    }
                    break;
                default:
                    buffer.Add(data);
                    break;
            }
        }
    }

    public class TelnetUnknownSequenceState : TelnetState
    {
        public TelnetUnknownSequenceState(TelnetOptionProcessor parent)
            : base(parent)
        {
        }

        public override void ProcessByte(byte data)
        {
            Parent.LogLine(data.ToString("d"));
            Parent.SetState<TelnetTextState>();
        }
    }

}

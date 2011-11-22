using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mirage.Telnet
{
    internal abstract class TelnetState
    {
        protected TelnetOptionProcessor Parent { get; private set; }

        public TelnetState(TelnetOptionProcessor parent)
        {
            this.Parent = parent;
        }

        public virtual void Enter(TelnetState previous, byte currentByte)
        {
        }

        public abstract void ProcessByte(byte data);
    }

    /// <summary>
    /// This is the default state where we process text
    /// </summary>
    internal class TelnetTextState : TelnetState
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

    internal class TelnetIACState : TelnetState
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
                case TelnetCodes.WILL:
                case TelnetCodes.WONT:
                    Parent.AppendLog(((TelnetCodes)data).ToString("g"));
                    Parent.SetState<TelnetNegotiateState>(data);
                    break;
                case TelnetCodes.SB:
                    Parent.AppendLog(((TelnetCodes)data).ToString("g"));
                    Parent.SetState<TelnetSubNegotiationState>();
                    break;
                default:
                    Parent.AppendLog("Unrecognized byte sequence " + data.ToString("d"));
                    Parent.SetState<TelnetUnknownSequenceState>(data);
                    break;
            }
        }
    }

    internal class TelnetNegotiateState : TelnetState
    {
        TelnetCodes currentCode;

        public TelnetNegotiateState(TelnetOptionProcessor parent)
            : base(parent)
        {
        }

        public override void Enter(TelnetState previous, byte currentByte)
        {
            if (!Enum.IsDefined(typeof(TelnetCodes), currentByte))
            {
                throw new ArgumentException(string.Format("Invalid TelnetCode, expecting DO, DONT, WILL, or WONT, received: {0:d}", currentByte), "currentByte");
            }
            currentCode = (TelnetCodes)currentByte;            
        }

        public override void ProcessByte(byte telopt)
        {
            // lookup the current state of the option
            TelnetOption option = Parent.LookupOption(telopt);

            // start processing...
            switch (currentCode)
            {
                // request to enable option on remote end or confirm DO
                case TelnetCodes.WILL:
                    switch (option.RemoteState)
                    {
                        case QState.Q_NO:
                            if (Parent.OptionSupport.IsSupportedRemotely(telopt))
                            {
                                option.RemoteState = QState.Q_YES;
                                // send confirmation
                                Parent.SendNegotiate(TelnetCommands.TELNET_DO, telopt);
                                option.OnOptionChanged(true, false);
                            }
                            else
                                // send rejection
                                Parent.SendNegotiate(TelnetCommands.TELNET_DONT, telopt);
                            break;
                        case QState.Q_WANTNO:
                            option.RemoteState = QState.Q_NO;
                            option.OnOptionChanged(false, false);
                            Parent.Logger.DebugFormat("DONT answered by WILL for Telopt: {0}", telopt);
                            break;
                        case QState.Q_WANTNO_OP:
                            option.RemoteState = QState.Q_YES;
                            Parent.Logger.DebugFormat("DONT answered by WILL for Telopt: {0}", telopt);
                            break;
                        case QState.Q_WANTYES:
                            option.RemoteState = QState.Q_YES;
                            option.OnOptionChanged(true, false);
                            break;
                        case QState.Q_WANTYES_OP:
                            option.RemoteState = QState.Q_WANTNO;
                            Parent.SendNegotiate(TelnetCommands.TELNET_DONT, telopt);
                            break;
                    }
                    break;

                // request to disable option on remote end, confirm DONT, reject DO
                case TelnetCodes.WONT:
                    switch (option.RemoteState)
                    {
                        case QState.Q_YES:
                            option.RemoteState = QState.Q_NO;
                            Parent.SendNegotiate(TelnetCommands.TELNET_DONT, telopt);
                            option.OnOptionChanged(false, false);
                            break;
                        case QState.Q_WANTNO:
                            option.RemoteState = QState.Q_NO;
                            option.OnOptionChanged(false, false); 
                            break;
                        case QState.Q_WANTNO_OP:
                            option.RemoteState = QState.Q_WANTYES;
                            break;
                        case QState.Q_WANTYES:
                        case QState.Q_WANTYES_OP:
                            option.RemoteState = QState.Q_NO;
                            option.OnOptionChanged(false, false); 
                            break;
                    }
                    break;

                // request to enable option on local end or confirm WILL
                case TelnetCodes.DO:
                    switch (option.LocalState)
                    {
                        case QState.Q_NO:
                            if (Parent.OptionSupport.IsSupportedLocally(telopt))
                            {
                                option.LocalState = QState.Q_YES;
                                // send confirmation
                                Parent.SendNegotiate(TelnetCommands.TELNET_WILL, telopt);
                                option.OnOptionChanged(true, true);
                            }
                            else
                                // send rejection
                                Parent.SendNegotiate(TelnetCommands.TELNET_WONT, telopt);
                            break;
                        case QState.Q_WANTNO:
                            option.LocalState = QState.Q_NO;
                            option.OnOptionChanged(false, true);
                            Parent.Logger.DebugFormat("WONT answered by DO for Telopt: {0}", telopt);
                            break;
                        case QState.Q_WANTNO_OP:
                            option.LocalState = QState.Q_YES;
                            Parent.Logger.DebugFormat("WONT answered by DO for Telopt: {0}", telopt);
                            break;
                        case QState.Q_WANTYES:
                            option.LocalState = QState.Q_YES;
                            option.OnOptionChanged(true, true);
                            break;
                        case QState.Q_WANTYES_OP:
                            option.LocalState = QState.Q_WANTNO;
                            Parent.SendNegotiate(TelnetCommands.TELNET_WONT, telopt);
                            break;
                    }
                    break;

                /* request to disable option on local end, confirm WONT, reject WILL */
                case TelnetCodes.DONT:
                    switch (option.LocalState)
                    {
                        case QState.Q_YES:
                            option.LocalState = QState.Q_NO;
                            Parent.SendNegotiate(TelnetCommands.TELNET_WONT, telopt);
                            option.OnOptionChanged(false, true);
                            break;
                        case QState.Q_WANTNO:
                            option.LocalState = QState.Q_NO;
                            option.OnOptionChanged(false, true);
                            break;
                        case QState.Q_WANTNO_OP:
                            option.LocalState = QState.Q_WANTYES;
                            break;
                        case QState.Q_WANTYES:
                        case QState.Q_WANTYES_OP:
                            option.LocalState = QState.Q_NO;
                            break;
                    }
                    break;
            }
            Parent.SetState<TelnetTextState>();
        }
    }

    internal class TelnetSubNegotiationState : TelnetState
    {
        List<byte> buffer = new List<byte>();
        bool lastWasIAC = false;

        public TelnetSubNegotiationState(TelnetOptionProcessor parent)
            : base(parent)
        {
        }

        public override void Enter(TelnetState previous, byte currentByte)
        {
            buffer.Clear();
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

    internal class TelnetUnknownSequenceState : TelnetState
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

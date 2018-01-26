using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Mirage.Core.IO.Net.Telnet
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

        public abstract Task ProcessByteAsync(byte data);
    }

    /// <summary>
    /// This is the default state where we process text
    /// </summary>
    internal class TelnetTextState : TelnetState
    {
        public TelnetTextState(TelnetOptionProcessor parent) : base(parent)
        {
        }

        public override Task ProcessByteAsync(byte data)
        {
            if (data == (byte)TelnetCommands.IAC)
            {
                Parent.SetState<TelnetIACState>();
                Parent.AppendLog("IAC");
            }
            else
            {
                Parent.AddProcessedByte(data);
            }
            return Task.CompletedTask;
        }
    }

    internal class TelnetIACState : TelnetState
    {
        public TelnetIACState(TelnetOptionProcessor parent)
            : base(parent)
        {
        }

        public override Task ProcessByteAsync(byte data)
        {
            switch ((TelnetCommands)data)
            {
                case TelnetCommands.IAC:
                    Parent.AddProcessedByte(data);
                    Parent.LogLine(((TelnetCommands)data).ToString("g"));
                    Parent.SetState<TelnetTextState>();
                    break;
                case TelnetCommands.DO:
                case TelnetCommands.DONT:
                case TelnetCommands.WILL:
                case TelnetCommands.WONT:
                    Parent.AppendLog(((TelnetCommands)data).ToString("g"));
                    Parent.SetState<TelnetNegotiateState>(data);
                    break;
                case TelnetCommands.SB:
                    Parent.AppendLog(((TelnetCommands)data).ToString("g"));
                    Parent.SetState<TelnetSubNegotiationState>();
                    break;
                default:
                    Parent.AppendLog("Unrecognized byte sequence " + data.ToString("d"));
                    Parent.SetState<TelnetUnknownSequenceState>(data);
                    break;
            }
            return Task.CompletedTask;
        }
    }

    internal class TelnetNegotiateState : TelnetState
    {
        TelnetCommands currentCode;
        
        public TelnetNegotiateState(TelnetOptionProcessor parent)
            : base(parent)
        {
        }

        public override void Enter(TelnetState previous, byte currentByte)
        {
            switch ((TelnetCommands)currentByte)
            {

                case TelnetCommands.DO:
                case TelnetCommands.DONT:
                case TelnetCommands.WILL:
                case TelnetCommands.WONT:
                    currentCode = (TelnetCommands) currentByte;
                    break;
                default:
                    throw new ArgumentException(string.Format("Invalid TelnetCode, expecting DO, DONT, WILL, or WONT, received: {0:d}", currentByte), "currentByte");
            }
        }

        public override async Task ProcessByteAsync(byte telopt)
        {
            Parent.AppendLog(telopt.ToString());

            // lookup the current state of the option
            TelnetOption option = Parent.LookupOption(telopt);

            switch (currentCode)
            {
                case TelnetCommands.DO:
                    await HandleDoAsync(option, telopt);
                    break;
                case TelnetCommands.DONT:
                    await HandleDontAsync(option, telopt);
                    break;
                case TelnetCommands.WILL:
                    await HandleWillAsync(option, telopt);
                    break;
                case TelnetCommands.WONT:
                    await HandleWontAsync(option, telopt);
                    break;
            }
            Parent.SetState<TelnetTextState>();
        }

        private async Task HandleWillAsync(TelnetOption option, byte telopt)
        {
            switch (option.RemoteState)
            {
                case QState.Q_NO:
                    if (Parent.OptionSupport.IsSupportedRemotely(telopt))
                    {
                        option.RemoteState = QState.Q_YES;
                        // send confirmation
                        await Parent.SendNegotiateAsync(TelnetCommands.DO, telopt);
                        option.OnOptionChanged(true, false);
                    }
                    else
                        // send rejection
                        await Parent.SendNegotiateAsync(TelnetCommands.DONT, telopt);
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
                    await Parent.SendNegotiateAsync(TelnetCommands.DONT, telopt);
                    break;
            }

        }
        private async Task HandleWontAsync(TelnetOption option, byte telopt)
        {
            switch (option.RemoteState)
            {
                case QState.Q_YES:
                    option.RemoteState = QState.Q_NO;
                    await Parent.SendNegotiateAsync(TelnetCommands.DONT, telopt);
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
        }
        private async Task HandleDoAsync(TelnetOption option, byte telopt)
        {
            switch (option.LocalState)
            {
                case QState.Q_NO:
                    if (Parent.OptionSupport.IsSupportedLocally(telopt))
                    {
                        option.LocalState = QState.Q_YES;
                        // send confirmation
                        await Parent.SendNegotiateAsync(TelnetCommands.WILL, telopt);
                        option.OnOptionChanged(true, true);
                    }
                    else
                        // send rejection
                        await Parent.SendNegotiateAsync(TelnetCommands.WONT, telopt);
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
                    await Parent.SendNegotiateAsync(TelnetCommands.WONT, telopt);
                    break;
            }
        }
        private async Task HandleDontAsync(TelnetOption option, byte telopt)
        {
            switch (option.LocalState)
            {
                case QState.Q_YES:
                    option.LocalState = QState.Q_NO;
                    await Parent.SendNegotiateAsync(TelnetCommands.WONT, telopt);
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

        public override Task ProcessByteAsync(byte data)
        {
            Parent.LogLine(data.ToString("d"));
            switch (data)
            {
                case (byte) TelnetCommands.IAC:
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
                case (byte) TelnetCommands.SE:
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
            return Task.CompletedTask;
        }
    }

    internal class TelnetUnknownSequenceState : TelnetState
    {
        public TelnetUnknownSequenceState(TelnetOptionProcessor parent)
            : base(parent)
        {
        }

        public override Task ProcessByteAsync(byte data)
        {
            Parent.LogLine(data.ToString("d"));
            Parent.SetState<TelnetTextState>();
            return Task.CompletedTask;
        }
    }

}

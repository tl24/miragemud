using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mirage.Telnet.Options;

namespace Mirage.Telnet
{
    /// <summary>
    /// Base event class for subnegotiation
    /// </summary>
    public class SubNegotiationEventArgs : System.EventArgs
    {
        public SubNegotiationEventArgs(byte option)
        {
            this.Option = option;
        }

        public byte Option { get; protected set; }
    }

    /// <summary>
    /// Event sent when the state of an option changes
    /// </summary>
    public class OptionStateChangedEventArgs : System.EventArgs    
    {
        public OptionStateChangedEventArgs(byte option, bool enabled, bool isLocal)
        {
            this.Option = option;
            this.Enabled = enabled;
            this.IsLocal = IsLocal;
        }

        public byte Option { get; protected set; }
        public bool Enabled { get; protected set; }
        public bool IsLocal { get; protected set; }
    }

    public class TelnetOptionProcessor
    {
        byte[] inputBuffer;
        byte[] outputBuffer;
        int outCount;
        private string logString = string.Empty;
        
        int index;

        private List<TelnetState> states = new List<TelnetState>();
        private List<TelnetOption> options = new List<TelnetOption>();
        private TelnetState currentState;

        public event EventHandler<SubNegotiationEventArgs> SubNegotiationOccurred;
        public event EventHandler<OptionStateChangedEventArgs> OptionStateChanged;

        public TelnetOptionProcessor(OptionSupportList optionSupport, IClient client, Castle.Core.Logging.ILogger logger)
        {
            this.OptionSupport = optionSupport;
            this.Client = client;
            this.Logger = logger;
            states.Add(new TelnetTextState(this));
            states.Add(new TelnetIACState(this));
            states.Add(new TelnetNegotiateState(this));
            states.Add(new TelnetSubNegotiationState(this));
            states.Add(new TelnetUnknownSequenceState(this));

            options.Add(new NawsOption(this));
            SetState<TelnetTextState>();
        }

        public IClient Client { get; private set; }

        protected internal Castle.Core.Logging.ILogger Logger { get; set; }

        internal OptionSupportList OptionSupport { get; private set; }

        public char[] ProcessBuffer(byte[] buffer)
        {
            return ProcessBuffer(buffer, buffer.Length);
        }
        public char[] ProcessBuffer(byte[] buffer, int length)
        {
            this.inputBuffer = buffer;
            outputBuffer = new byte[length];
            outCount = 0;
            for (index = 0; index < length; index++)
            {
                currentState.ProcessByte(inputBuffer[index]);
            }
            if (outCount > 0)
                return Encoding.ASCII.GetChars(outputBuffer, 0, outCount);
            else
                return new char[0];
        }

        internal void AppendLog(string name)
        {
            if (Logger.IsDebugEnabled)
            {
                logString += name;
                logString += " ";
            }
        }

        internal void LogLine(string text)
        {
            AppendLog(text);
            LogLine();
        }

        internal void LogLine()
        {
            if (!string.IsNullOrEmpty(logString))
            {
                Logger.DebugFormat("Recieved IAC sequence: {0}", logString);
                logString = "";
            }
        }

        /// <summary>
        /// Set the current state where the data byte does not matter
        /// </summary>
        /// <typeparam name="StateType">the new state</typeparam>
        internal void SetState<StateType>()
        {
            SetState<StateType>(0);
        }

        /// <summary>
        /// Set the current state, passing in the byte of data that caused us to enter this state
        /// </summary>
        /// <typeparam name="StateType">the new state type</typeparam>
        /// <param name="currentByte">the data byte that caused us to enter the new state</param>
        internal void SetState<StateType>(byte currentByte)
        {
            TelnetState state = states.Find((s) => (s is StateType));
            if (state == null)
                throw new ArgumentException("Unrecognized telnet state: " + typeof(StateType));
            var prevState = currentState;
            currentState = state;
            currentState.Enter(prevState, currentByte);
        }

        internal TelnetOption LookupOption(byte optionValue)
        {
            TelnetOption option = options.Find((o) => (o.OptionValue == optionValue));
            if (option == null)
                option = new TelnetOption(this, optionValue);
            return option;
        }
        /// <summary>
        /// Adds a byte of data to the output buffer
        /// </summary>
        /// <param name="data"></param>
        internal void AddOutputByte(byte data)
        {
            outputBuffer[outCount++] = data;
        }

        internal void SendBytes(byte[] data)
        {
            Client.Write(data);
        }

        internal void OnSubNegotiationOccurred(SubNegotiationEventArgs args)
        {
            if (SubNegotiationOccurred != null)
                SubNegotiationOccurred(this, args);
        }

        internal void OnOptionStateChanged(OptionStateChangedEventArgs args)
        {
            if (OptionStateChanged != null)
                OptionStateChanged(this, args);
        }

        internal void SendNegotiate(byte cmd, byte telopt)
        {
            Logger.DebugFormat("Sending IAC {0:g} {1:g}", cmd, telopt);
            SendBytes(new byte[] { TelnetCommands.TELNET_IAC, cmd, telopt });
        }

        /// <summary>
        /// Start negotiation for an telnet option
        /// </summary>
        /// <param name="cmd">the command: DO, DONT, WILL, WONT</param>
        /// <param name="telopt">The telnet option</param>
        public void TelnetNegotiate(byte cmd, byte telopt)
        {
            // get current option states
            TelnetOption option = LookupOption(telopt);
            switch (cmd)
            {
                // advertise willingess to support an option
                case TelnetCommands.TELNET_WILL:
                    switch (option.LocalState)
                    {
                        case QState.Q_NO:
                            option.LocalState = QState.Q_WANTYES;
                            SendNegotiate(TelnetCommands.TELNET_WILL, telopt);
                            break;
                        case QState.Q_WANTNO:
                            option.LocalState = QState.Q_WANTNO_OP;
                            break;
                        case QState.Q_WANTYES_OP:
                            option.LocalState = QState.Q_WANTYES;
                            break;
                    }
                    break;

                // force turn-off of locally enabled option
                case TelnetCommands.TELNET_WONT:
                    switch (option.LocalState)
                    {
                        case QState.Q_YES:
                            option.LocalState = QState.Q_WANTNO;
                            SendNegotiate(TelnetCommands.TELNET_WONT, telopt);
                            break;
                        case QState.Q_WANTYES:
                            option.LocalState = QState.Q_WANTYES_OP;
                            break;
                        case QState.Q_WANTNO_OP:
                            option.LocalState = QState.Q_WANTNO;
                            break;
                    }
                    break;

                // ask remote end to enable an option
                case TelnetCommands.TELNET_DO:
                    switch (option.RemoteState)
                    {
                        case QState.Q_NO:
                            option.RemoteState = QState.Q_WANTYES;
                            SendNegotiate(TelnetCommands.TELNET_DO, telopt);
                            break;
                        case QState.Q_WANTNO:
                            option.RemoteState = QState.Q_WANTNO_OP;
                            break;
                        case QState.Q_WANTYES_OP:
                            option.RemoteState = QState.Q_WANTYES;
                            break;
                    }
                    break;

                // demand remote end disable an option
                case TelnetCommands.TELNET_DONT:
                    switch (option.RemoteState)
                    {
                        case QState.Q_YES:
                            option.RemoteState = QState.Q_WANTNO;
                            SendNegotiate(TelnetCommands.TELNET_DONT, telopt);
                            break;
                        case QState.Q_WANTYES:
                            option.RemoteState = QState.Q_WANTYES_OP;
                            break;
                        case QState.Q_WANTNO_OP:
                            option.RemoteState = QState.Q_WANTNO;
                            break;
                    }
                    break;
            }

        }
    }

}

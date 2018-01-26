using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Mirage.Core.IO.Net.Telnet.Options;
using System.Threading.Tasks;

namespace Mirage.Core.IO.Net.Telnet
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

    public class TelnetOptionProcessor : IDisposable
    {
        private byte[] processedInput;
        private int processedInputCount;
        private Stream inputStream;
        private Stream outputStream;
        private Encoding encoding = Encoding.ASCII;

        private static readonly char[] emptyChars = new char[0];
        private static readonly byte[] subNegotiateStart = new byte [] { (byte)TelnetCommands.IAC, (byte)TelnetCommands.SB };
        private static readonly byte[] subNegotiateEnd = new byte[] { (byte)TelnetCommands.IAC, (byte)TelnetCommands.SE };
        private string logString = string.Empty;
        
        int index;

        private List<TelnetState> states = new List<TelnetState>();
        private List<TelnetOption> options = new List<TelnetOption>();
        private TelnetState currentState;

        public event EventHandler<SubNegotiationEventArgs> SubNegotiationOccurred;
        public event EventHandler<OptionStateChangedEventArgs> OptionStateChanged;

        public TelnetOptionProcessor(OptionSupportList optionSupport, Stream ioStream, Castle.Core.Logging.ILogger logger)
            : this(optionSupport, ioStream, ioStream, logger)
        {
        }

        public TelnetOptionProcessor(OptionSupportList optionSupport, Stream input, Stream output, Castle.Core.Logging.ILogger logger)
        {
            this.OptionSupport = optionSupport;
            this.inputStream = input;
            this.outputStream = output;
            this.Logger = logger;
            InitializeStates();

            InitializeOptions();
            SetState<TelnetTextState>();
        }

        private void InitializeOptions()
        {
            options.Add(new NawsOption(this));
            options.Add(new EnvironOption(this));
            options.Add(new TermTypeOption(this));
        }

        private void InitializeStates()
        {
            states.Add(new TelnetTextState(this));
            states.Add(new TelnetIACState(this));
            states.Add(new TelnetNegotiateState(this));
            states.Add(new TelnetSubNegotiationState(this));
            states.Add(new TelnetUnknownSequenceState(this));
        }

        protected internal Castle.Core.Logging.ILogger Logger { get; set; }

        internal OptionSupportList OptionSupport { get; private set; }

        public async Task<char[]> ReadAsync(int availableBytes)
        {
            // TODO: reuse buffers
            byte[] inBuf = new byte[availableBytes];
            int bRead = 0;
            // TODO: not sure why we were locking, make sure there isn't concurrent access
            //lock (inputStream)
            //{
                bRead = await inputStream.ReadAsync(inBuf, 0, inBuf.Length);
            //}
            return ProcessInput(inBuf, bRead);
        }

        protected char[] ProcessInput(byte[] buffer)
        {
            return ProcessInput(buffer, buffer.Length);
        }
        protected char[] ProcessInput(byte[] buffer, int length)
        {
            byte[] inputBuffer = buffer;
            processedInput = new byte[length];
            processedInputCount = 0;
            for (index = 0; index < length; index++)
            {
                currentState.ProcessByteAsync(inputBuffer[index]);
            }
            if (processedInputCount > 0)
                return encoding.GetChars(processedInput, 0, processedInputCount);
            else
                return emptyChars;
        }

        /// <summary>
        /// Convert entire byte array into the string using the proper encoding
        /// </summary>
        /// <param name="bytes">byte array to convert</param>
        /// <returns>string</returns>
        internal string BytesToString(byte[] bytes)
        {
            return BytesToString(bytes, 0, bytes.Length);
        }

        /// <summary>
        /// Convert the specified number bytes into the string using the proper encoding
        /// </summary>
        /// <param name="bytes">byte array</param>
        /// <param name="length">number of bytes to convert</param>
        /// <returns>string</returns>
        internal string BytesToString(byte[] bytes, int length)
        {
            return BytesToString(bytes, 0, length);
        }

        /// <summary>
        /// Convert the specified bytes into the string using the proper encoding
        /// </summary>
        /// <param name="bytes">byte array</param>
        /// <param name="offset">the starting offset to start the conversion</param>
        /// <param name="length">number of bytes to convert</param>
        /// <returns>string</returns>
        internal string BytesToString(byte[] bytes, int offset, int length)
        {
            return encoding.GetString(bytes, offset, length);
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
            TelnetOption option = options.Find((o) => (o.OptionCode == optionValue));
            if (option == null)
            {
                option = new TelnetOption(this, optionValue);
                options.Add(option);
            }
            return option;
        }
        /// <summary>
        /// Adds a byte of data to the output buffer
        /// </summary>
        /// <param name="data"></param>
        internal void AddProcessedByte(byte data)
        {
            processedInput[processedInputCount++] = data;
        }

        public async Task SendTestConnectedAsync()
        {
            // for now send null...could send NOP
            await WriteRawAsync(new byte[1]);
        }

        public async Task WriteAsync(string text)
        {
            // for now, don't worry about escaping
            byte[] data = encoding.GetBytes(text);
            await WriteRawAsync(data);
        }

        public async Task WriteRawAsync(byte[] data)
        {
            //lock (outputStream)
            //{
                //TODO: buffering?
                await outputStream.WriteAsync(data, 0, data.Length);
            //}
        }

        internal async Task SendSubNegotiate(byte[] data)
        {
            await WriteRawAsync(subNegotiateStart);
            await WriteRawAsync(data);
            await WriteRawAsync(subNegotiateEnd);
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

        internal async Task SendNegotiateAsync(TelnetCommands cmd, byte telopt)
        {
            Logger.DebugFormat("Sending IAC {0:g} {1:g}", cmd, telopt);
            await WriteRawAsync(new byte[] { (byte) TelnetCommands.IAC, (byte) cmd, telopt });
        }

        internal void OnProtocolError(string errorText)
        {
            LogLine(errorText);
        }

        /// <summary>
        /// Start negotiation for an telnet option
        /// </summary>
        /// <param name="cmd">the command: DO, DONT, WILL, WONT</param>
        /// <param name="telopt">The telnet option</param>
        public async Task TelnetNegotiateAsync(TelnetCommands cmd, byte telopt)
        {
            // get current option states
            TelnetOption option = LookupOption(telopt);
            switch (cmd)
            {
                // advertise willingess to support an option
                case TelnetCommands.WILL:
                    switch (option.LocalState)
                    {
                        case QState.Q_NO:
                            option.LocalState = QState.Q_WANTYES;
                            await SendNegotiateAsync(TelnetCommands.WILL, telopt);
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
                case TelnetCommands.WONT:
                    switch (option.LocalState)
                    {
                        case QState.Q_YES:
                            option.LocalState = QState.Q_WANTNO;
                            await SendNegotiateAsync(TelnetCommands.WONT, telopt);
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
                case TelnetCommands.DO:
                    switch (option.RemoteState)
                    {
                        case QState.Q_NO:
                            option.RemoteState = QState.Q_WANTYES;
                            await SendNegotiateAsync(TelnetCommands.DO, telopt);
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
                case TelnetCommands.DONT:
                    switch (option.RemoteState)
                    {
                        case QState.Q_YES:
                            option.RemoteState = QState.Q_WANTNO;
                            await SendNegotiateAsync(TelnetCommands.DONT, telopt);
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

        #region IDisposable Members

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        #endregion
    }

}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MirageMUD.Core;

namespace Mirage.Core.IO
{
    public class TelnetOptionProcessor
    {
        byte[] inputBuffer;
        byte[] outputBuffer;
        int outCount;
        private string logString = string.Empty;
        Castle.Core.Logging.ILogger logger;
        int index;

        private List<TelnetState> states = new List<TelnetState>();
        private List<TelnetOption> options = new List<TelnetOption>();
        private TelnetState currentState;

        public TelnetOptionProcessor(ITextClient client, Castle.Core.Logging.ILogger logger)
        {
            this.Client = client;
            this.logger = logger;
            states.Add(new TelnetTextState(this));
            states.Add(new TelnetIACState(this));
            states.Add(new TelnetDoDontState(this));
            states.Add(new TelnetWillWontState(this));
            states.Add(new TelnetSubNegotiationState(this));
            states.Add(new TelnetUnknownSequenceState(this));

            options.Add(new EchoOption(this));
            options.Add(new NawsOption(this));
            SetState<TelnetTextState>();
        }

        public ITextClient Client { get; private set; }

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

        /// <summary>
        /// Looks behind in the byte sequence
        /// </summary>
        /// <param name="numPlaces">the number of places to look back</param>
        /// <returns>the byte at the position</returns>
        public byte LookBehind(int numPlaces)
        {
            int nextIndex = index - numPlaces;
            if (nextIndex >= 0)
                return inputBuffer[nextIndex];
            else
                return 0;
        }

        public void AppendLog(string name)
        {
            if (logger.IsDebugEnabled)
            {
                logString += name;
                logString += " ";
            }
        }

        public void LogLine(string text)
        {
            AppendLog(text);
            LogLine();
        }

        public void LogLine()
        {
            if (!string.IsNullOrEmpty(logString))
            {
                logger.DebugFormat("Recieved IAC sequence: {0}", logString);
                logString = "";
            }
        }

        public void SetState<StateType>()
        {
            TelnetState state = states.Find((s) => (s is StateType));
            if (state == null)
                throw new ArgumentException("Unrecognized telnet state: " + typeof(StateType));
            currentState = state;
        }

        public TelnetOption LookupOption(byte optionValue)
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
        public void AddOutputByte(byte data)
        {
            outputBuffer[outCount++] = data;
        }

        public void SendBytes(byte[] data)
        {
            Client.Write(data);
        }
    }

}

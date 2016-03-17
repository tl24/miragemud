using System;
using System.Net.Sockets;
using Mirage.Core.Collections;
using Mirage.Core.IO.Net.Telnet;
using Mirage.Core.IO.Net.Telnet.Options;
using System.Collections.Concurrent;

namespace Mirage.Core.IO.Net
{
    public class TextConnection : SocketConnection, IConnection
    {
        protected NetworkStream socketStream;

        /// <summary>
        ///     The lines that have been read from the socket
        /// </summary>
        protected ConcurrentQueue<string> inputQueue;

        /// <summary>
        ///     Buffer to hold input until it forms a complete line
        /// </summary>
        protected char[] inputBuffer;

        /// <summary>
        ///     The number of characters that have been Put in the buffer
        /// </summary>
        protected int bufferLength;

        public TextConnection(TcpClient client)
            : base(client)
        {
            socketStream = _client.GetStream();
            inputQueue = new ConcurrentQueue<string>();
            outputQueue = new ConcurrentQueue<string>();
            Options = new TextClientOptions();
            inputBuffer = new char[512];
            bufferLength = 0;
            //for now, just call this, not sure we need to expose it
            Initialize();
        }

        /// <summary>
        ///     The lines that are waiting to be written to the socket
        /// </summary>
        private ConcurrentQueue<string> outputQueue;

        protected bool checkDisconnect = false;

        private TelnetOptionProcessor tnHandler;

        private void Initialize()
        {
            var telnetOpts = new OptionSupportList(new[] { 
                new OptionSupportEntry(OptionCodes.NAWS, true, false),
                new OptionSupportEntry(OptionCodes.ECHO, true, true),
                new OptionSupportEntry(OptionCodes.NEW_ENVIRON, true, true),
                new OptionSupportEntry(OptionCodes.TTYPE, true, true)
            });
            tnHandler = new TelnetOptionProcessor(telnetOpts, socketStream, Logger);
            tnHandler.SubNegotiationOccurred += new EventHandler<SubNegotiationEventArgs>(tnHandler_SubNegotiationOccurred);
            tnHandler.OptionStateChanged += new EventHandler<OptionStateChangedEventArgs>(tnHandler_OptionStateChanged);
            tnHandler.TelnetNegotiate(TelnetCommands.DO, OptionCodes.NAWS);
            tnHandler.TelnetNegotiate(TelnetCommands.DO, OptionCodes.TTYPE);
            tnHandler.TelnetNegotiate(TelnetCommands.WILL, OptionCodes.SGA);
            tnHandler.TelnetNegotiate(TelnetCommands.WILL, OptionCodes.ECHO);
            //Write(new StringMessage(MessageType.Information, "Newline", "\r\n"));            
        }

        void tnHandler_OptionStateChanged(object sender, OptionStateChangedEventArgs e)
        {
            /*
            if (e.Option == OptionCodes.TTYPE && e.Enabled)
            {
                this.tnHandler.SendRawBytes(
            }
             */
        }

        void tnHandler_SubNegotiationOccurred(object sender, SubNegotiationEventArgs e)
        {
            switch (e.Option)
            {
                case OptionCodes.NAWS:
                    NawsEventArgs ne = (NawsEventArgs) e;
                    Options.WindowHeight = ne.Height;
                    Options.WindowWidth = ne.Width;
                    break;
                case OptionCodes.TTYPE:
                    TermTypeEventArgs tte = (TermTypeEventArgs)e;
                    if (tte.Option == TelnetSubOptionCodes.TELNET_TTYPE_IS)
                    {
                        Options.TerminalType = tte.Name;
                    }
                    break;
            }
        }

        public TextClientOptions Options { get; private set; }

        /// <summary>
        ///     Read from the descriptor.  Returns True if successful.
        ///     Populates an internal buffer, which can be read by read_from_buffer.
        /// </summary>
        public override void ReadInput() {
            int available = _client.Available;

            if (bufferLength == inputBuffer.Length)
            {
                Array.Resize<char>(ref inputBuffer, inputBuffer.Length * 2);
            }
            if (available == 0)
                available = 1;

            int nRead = ReadFromSocket(available);

            // if we're trying to read, the socket told us there is something to read so there shouldn't be 0 bytes
            checkDisconnect = nRead == 0;

            char prev = '\0';
            int endPos = -1;
            int endLen = 0;
            for (int i = bufferLength; i < inputBuffer.Length && i < bufferLength + nRead; i++)
            {
                char cur = inputBuffer[i];
                if (cur == '\n') {
                    if (prev == '\r') {
                        endPos = i - 1;
                        endLen = 2;
                    } else {
                        endPos = i;
                        endLen = 1;
                    }
                    break;
                } else if (cur == '\r') {
                } else if (prev == '\r') {
                    endPos = i - 1;
                    endLen = 1;
                    break;
                }
                prev = cur;
            }

            bufferLength += nRead;
            string line = null;
            if (endPos != -1)
            {
                line = new string(inputBuffer, 0, endPos);
                line = line.Trim();
                Array.Copy(inputBuffer, endPos + endLen, inputBuffer, 0, bufferLength - endPos - endLen);
                bufferLength -= endPos + endLen;
            }

            if (line == null)
            {
                return;
            }

            inputQueue.Enqueue(line);            
        }

        private int ReadFromSocket(int available)
        {
            available = Math.Min(inputBuffer.Length - bufferLength, available);
            char[] outBuf = tnHandler.Read(available);
            Array.Copy(outBuf, 0, inputBuffer, bufferLength, outBuf.Length);
            return outBuf.Length;
        }

        public bool TryGetInput(out string input)
        {
            input = null;
            if (_closed == 0 && inputQueue.TryDequeue(out input)) {
                return true;
            } else {
                return false;
            }
        }

        public void Write(string message)
        {
            outputQueue.Enqueue(message);
        }

        /// <summary>
        ///     Process the output waiting in the output buffer.  This
        /// Data will be sent to the socket.
        /// </summary>
        public override void FlushOutput()
        {
            bool bProcess = false;
            string data;
            while (outputQueue.TryDequeue(out data))
            {
                tnHandler.Write(data);
                bProcess = true;
            }
            if (bProcess)
            {
                ;
            }
            else
            {
                if (IsOpen && checkDisconnect)
                {
                    // if we read 0 bytes in the Read function, then try to send something to see if
                    // the connection is closed
                    tnHandler.SendTestConnected();
                    //this.socketStream.Write(new byte[1],0,1);
                    if (!this._client.Connected)
                        Close();
                }
            }
        }
    }
}

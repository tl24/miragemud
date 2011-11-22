using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Net.Sockets;
using System.IO;
using Mirage.Core.Data;
using Mirage.Core.Command;
using Mirage.Core.Communication;
using Mirage.Core.Util;
using MirageMUD.Core;

namespace Mirage.Core.IO
{

    /// <summary>
    ///     Handles Server for a player
    /// </summary>
    public class TextClient : ClientBase
    {

        protected NetworkStream socketStream;

        /// <summary>
        ///     A reader for the tcp client's stream
        /// </summary>
        protected StreamReader reader;

        /// <summary>
        ///     A writer for the tcp client's stream
        /// </summary>
        protected StreamWriter writer;

        /// <summary>
        ///     The lines that have been read from the socket
        /// </summary>
        protected ISynchronizedQueue<string> inputQueue;

        /// <summary>
        ///     The incomming line to be processed
        /// </summary>
        protected string _inputLine;

        /// <summary>
        ///     Buffer to hold input until it forms a complete line
        /// </summary>
        protected char[] inputBuffer;

        /// <summary>
        ///     The number of characters that have been Put in the buffer
        /// </summary>
        protected int bufferLength;


        /// <summary>
        ///     The last Command read
        /// </summary>
        protected string lastRead;

        /// <summary>
        ///     The lines that are waiting to be written to the socket
        /// </summary>
        private ISynchronizedQueue<OutputData> outputQueue;

        protected bool checkDisconnect = false;

        private TelnetHandler tnHandler;

        /// <summary>
        ///     Create a descriptor to read and write to the given
        /// tcp client (Socket)
        /// </summary>
        /// <param name="client"> the client to read and write from</param>
        public TextClient(TcpClient client) : base(client)
        {
            socketStream = _client.GetStream();
            reader = new StreamReader(socketStream);
            writer = new StreamWriter(socketStream);
            inputQueue = new SynchronizedQueue<string>();
            outputQueue = new SynchronizedQueue<OutputData>();
            Write(new StringMessage(MessageType.Information, "Newline", "\r\n"));
            inputBuffer = new char[512];
            bufferLength = 0;
            
        }

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

            //string line = reader.ReadLine();
            //string line = new string(buf, 0, nRead);
            if (line == null)
            {
                return;
            }
            // Insert space for Command holder if necessary
            //if (line.Length == 0)
            //{
            //    line = " ";
            //}
            inputQueue.Enqueue(line);            
        }

        private int ReadFromSocket(int available)
        {
            if (tnHandler == null) {
                tnHandler = new TelnetHandler(this, Logger);
            }
            available = Math.Min(inputBuffer.Length - bufferLength, available);
            byte[] inBuf = new byte[available];
            int bRead = socketStream.Read(inBuf, 0, inBuf.Length);
            char[] outBuf = tnHandler.ProcessBuffer(inBuf, bRead);
            Array.Copy(outBuf, 0, inputBuffer, bufferLength, outBuf.Length);
            return outBuf.Length;
        }

        public override void ProcessInput()
        {
            string input;
            if (_closed == 0 && inputQueue.TryDequeue(out input)) {
                CommandRead = true;
                if (LoginHandler != null)
                {
                    LoginHandler.HandleInput(input);
                }
                else if (input.Trim().Length > 0)
                {
                    Interpreter.ExecuteCommand(Player, input);
                }
            }
        }

        /// <summary>
        ///    Transfer input from buffer to INCOMM so a Command can be processed. 
        /// </summary>
        private void ReadFromBuffer() {
            // If we have a line already, return
            if (_inputLine != null)
            {
                return;
            }
            _commandRead = false;
            string line = inputQueue.Dequeue();
            // Substitute for last Command with '!'
            if (line.Equals("!"))
            {
                _inputLine = lastRead;
            }
            else
            {
                _inputLine = line;
                lastRead = _inputLine;
            }

            if (_inputLine != null && _inputLine.Length > 0)
            {
                _commandRead = true;
            }
        }

        /// <summary>
        /// Write the specified text to the descriptors output buffer. 
        /// </summary>
        public override void Write(IMessage message)
        {
            if (_closed == 0)
            {
                outputQueue.Enqueue(new OutputData(message.Render()));
                OutputWritten = true;
            }
        }

        private void WriteBytes(byte[] bytes)
        {
            if (_closed == 0)
            {
                outputQueue.Enqueue(new OutputData(bytes));
            }
        }
        /// <summary>
        ///     Process the output waiting in the output buffer.  This
        /// Data will be sent to the socket.
        /// </summary>
        public override void FlushOutput()
        {
            bool bProcess = false;
            while (outputQueue.Count > 0) {
                OutputData data = outputQueue.Dequeue();
                byte[] bytes = data.Bytes;
                this.socketStream.Write(bytes, 0, bytes.Length);
                bProcess = true;
            }
            if (bProcess)
            {
                writer.Flush();
            }
            else
            {
                if (IsOpen && checkDisconnect)
                {
                    // if we read 0 bytes in the Read function, then try to send something to see if
                    // the connection is closed
                    this._client.Client.Send(new byte[1]);
                    if (!this._client.Connected)
                        Close();
                }
            }
        }

        public override void Dispose()
        {
            base.Dispose();
            reader.Dispose();
            writer.Dispose();
        }

        internal struct OutputData
        {
            private object data;
            private bool isString;
            public OutputData(string data)
            {
                this.data = data;
                this.isString = true;
            }
            public OutputData(byte[] data)
            {
                this.data = data;
                this.isString = false;
            }

            public OutputData(object data, bool isString)
            {
                this.data = data;
                this.isString = isString;
            }

            public bool IsString
            {
                get { return this.isString; }
            }

            public byte[] Bytes
            {
                get {
                    if (IsString)
                        return Encoding.ASCII.GetBytes((string)this.data);
                    else
                        return (byte[])this.data; 
                }
            }
        }

        internal class TelnetHandler
        {
            TextClient client;
            byte[] inputBuffer;
            byte[] outputBuffer;
            Action<byte> currentProcessor;
            int outCount;
            private string logString = string.Empty;
            Castle.Core.Logging.ILogger logger;
            internal TelnetHandler(TextClient client, Castle.Core.Logging.ILogger logger)
            {
                this.client = client;
                this.logger = logger;
                this.currentProcessor = ProcessText;
            }

            internal char[] ProcessBuffer(byte[] buffer, int length)
            {
                this.inputBuffer = buffer;
                outputBuffer = new byte[length];
                outCount = 0;
                for (int i = 0; i < length; i++)
                {
                    currentProcessor(inputBuffer[i]);
                }
                if (outCount > 0)
                    return Encoding.ASCII.GetChars(outputBuffer, 0, outCount);
                else
                    return new char[0];
            }

            private void ProcessText(byte data)
            {
                if (data == (byte)TelnetCodes.IAC)
                {
                    currentProcessor = ProcessIAC;
                    logString += "IAC ";
                }
                else
                {
                    AddOutputByte(data);
                }
            }

            private void ProcessIAC(byte data)
            {
                switch ((TelnetCodes)data)
                {
                    case TelnetCodes.IAC:
                        AddOutputByte(data);
                        logString += ((TelnetCodes)data).ToString("g") + " ";
                        LogOutput();
                        currentProcessor = ProcessText;
                        break;
                    case TelnetCodes.DO:
                    case TelnetCodes.DONT:
                        logString += ((TelnetCodes)data).ToString("g") + " ";
                        currentProcessor = ProcessDoDont;
                        break;
                    case TelnetCodes.WILL:
                    case TelnetCodes.WONT:
                        logString += ((TelnetCodes)data).ToString("g") + " ";
                        currentProcessor = ProcessWillWont;
                        break;
                    default:
                        logString += "Unrecognized byte sequence " + data.ToString("d") + " ";
                        currentProcessor = ProcessNextByte;
                        break;
                }
            }

            private void ProcessNextByte(byte data)
            {
                logString += data.ToString("d");
                LogOutput();
                currentProcessor = ProcessText;
            }

            private void ProcessDoDont(byte data)
            {
                // for now, we always Wont
                logString += data.ToString("d") + " ";
                LogOutput();
                logger.DebugFormat("Sending IAC WONT {0}", data);
                client.WriteBytes(new byte[] { (byte)TelnetCodes.IAC, (byte)TelnetCodes.WONT, data });
                currentProcessor = ProcessText;
            }

            private void ProcessWillWont(byte data)
            {
                // for now, we always Dont
                logString += data.ToString("d") + " ";
                LogOutput();
                logger.DebugFormat("Sending IAC DONT {0}", data);
                client.WriteBytes(new byte[] { (byte)TelnetCodes.IAC, (byte)TelnetCodes.DONT, data });
                currentProcessor = ProcessText;
            }

            private void LogOutput()
            {
                if (!string.IsNullOrEmpty(logString))
                {
                    logger.DebugFormat("Recieved IAC sequence: {0}", logString);
                    logString = "";
                }
            }

            private void AddOutputByte(byte data)
            {
                outputBuffer[outCount++] = data;
            }
        }
    }
}

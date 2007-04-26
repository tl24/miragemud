using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Net.Sockets;
using System.IO;
using Shoop.Data;
using Shoop.Command;

namespace Shoop.IO
{

    /// <summary>
    ///     The connection state for a player.  A player goes through
    /// various states before they are completely logged in
    /// </summary>
    public enum ConnectedState
    {
        /// <summary>
        ///     The player is connecting
        /// </summary>
        Connecting,
        /// <summary>
        ///     The player is idle
        /// </summary>
        Idle,
        /// <summary>
        ///     The player is playing, completely logged in and not idle
        /// </summary>
        Playing
    }

    /// <summary>
    ///     Handles Server for a player
    /// </summary>
    public class Descriptor
    {
        /// <summary>
        ///     The player object attached to this descriptor
        /// </summary>
        private Player _player;

        /// <summary>
        ///     The nanny for this descriptor, if applicable
        /// </summary>
        private Nanny _nanny;

        /// <summary>
        ///     A reference to the tcp client (socket) that this description
        /// reads and writes from
        /// </summary>
        private TcpClient _client;

        /// <summary>
        ///     A reader for the tcp client's stream
        /// </summary>
        private StreamReader reader;

        /// <summary>
        ///     A writer for the tcp client's stream
        /// </summary>
        private StreamWriter writer;

        /// <summary>
        ///     The lines that have been read from the socket
        /// </summary>
        private Queue<string> inputQueue;

        /// <summary>
        ///     The lines that are waiting to be written to the socket
        /// </summary>
        private Queue<string> outputQueue;

        /// <summary>
        ///     The incomming line to be processed
        /// </summary>
        private string _inputLine;

        /// <summary>
        ///     Buffer to hold input until it forms a complete line
        /// </summary>
        private char[] inputBuffer;

        /// <summary>
        ///     The number of characters that have been put in the buffer
        /// </summary>
        private int bufferLength;


        /// <summary>
        ///     The last Command read
        /// </summary>
        private string lastRead;

        /// <summary>
        ///     The list of people snooping this connection
        /// </summary>
        private IList<Descriptor> snoopers;

        /// <summary>
        ///     The stage of connection that this descriptor is at
        /// </summary>
        private ConnectedState _state;

        /// <summary>
        ///     Indicates that a Command was read this cycle
        /// </summary>
        private bool _commandRead;

        /// <summary>
        ///     Create a descriptor to read and write to the given
        /// tcp client (Socket)
        /// </summary>
        /// <param name="client"> the client to read and write from</param>
        public Descriptor(TcpClient client)
        {
            this._client = client;
            NetworkStream stm = _client.GetStream();
            reader = new StreamReader(stm);
            writer = new StreamWriter(stm);            
            inputQueue = new Queue<string>();
            outputQueue = new Queue<string>();
            outputQueue.Enqueue("\n\r");
            snoopers = new List<Descriptor>();
            inputBuffer = new char[512];
            bufferLength = 0;
        }

        /// <summary>
        ///     The stage of connection that this descriptor is at
        /// </summary>
        public ConnectedState state
        {
            get { return _state; }
            set { _state = value; }
        }

        /// <summary>
        ///     The player object attached to this descriptor
        /// </summary>
        public Player player
        {
            get { return _player; }
            set { _player = value; }
        }

        /// <summary>
        ///     The nanny for this descriptor, can be null
        /// </summary>
        public Nanny nanny
        {
            get { return _nanny; }
            set { _nanny = value; }
        }

        /// <summary>
        ///     The socket for this description
        /// </summary>
        public TcpClient client
        {
            get { return _client; }
            set { _client = value; }
        }

        /// <summary>
        ///     The current line of input
        /// </summary>
        public string inputLine
        {
            get { return _inputLine; }
            set { _inputLine = value; }
        }

        /// <summary>
        ///     Read from the descriptor.  Returns True if successful.
        ///     Populates an internal buffer, which can be read by read_from_buffer.
        /// </summary>
        public bool read() {
            int available = _client.Available;

            if (bufferLength == inputBuffer.Length)
            {
                char[] newBuffer = new char[inputBuffer.Length * 2];
                Array.Copy(inputBuffer, newBuffer, bufferLength);
                inputBuffer = newBuffer;
            }

            int nRead = reader.ReadBlock(inputBuffer, bufferLength, Math.Min(inputBuffer.Length - bufferLength, _client.Available));

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
                return false;
            }
            // Insert space for Command holder if necessary
            if (line.Length == 0)
            {
                line = " ";
            }
            inputQueue.Enqueue(line);
            return true;
        }

        /// <summary>
        ///    Transfer input from buffer to INCOMM so a Command can be processed. 
        /// </summary>
        public void readFromBuffer() {
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
        ///     Indicates that a Command was read this cycle
        /// </summary>
        public bool commandRead
        {
            get { return _commandRead; }
            set { _commandRead = value; }
        }

        /// <summary>
        /// Write the specified text to the descriptors output buffer. 
        /// If snoop parameter is true, write to any descriptors snooping
        /// this one as well, otherwise write to this descriptor only.
        /// </summary>
        public void writeToBuffer(string text, bool snoop) {
            outputQueue.Enqueue(text);

            if (snoop) {
                foreach (Descriptor desc in snoopers) {
                    desc.writeToBuffer(this.player.title, false);
                    desc.writeToBuffer(" > ", false);
                    desc.writeToBuffer(text, false);
                }
            }
        }

        /// <summary>
        /// Write the specified text to the descriptors output buffer. 
        /// </summary>
        public void writeToBuffer(string text)
        {
            writeToBuffer(text, true);
        }

        /// <summary>
        ///     Process the output waiting in the output buffer.  This
        /// Data will be sent to the socket.
        /// </summary>
        public void processOutputBuffer()
        {
            bool bProcess = false;
            while (outputQueue.Count > 0) {
                string line = outputQueue.Dequeue();
                write(line);
                bProcess = true;
            }
            if (bProcess)
            {
                writer.Flush();
            }
        }

        /// <summary>
        ///    Write to the descriptor.  Similar to print statement.
        /// </summary>
        /// <param name="text">the text to write</param>
        public void write(string text) {
            writer.Write(text);
        }

        /// <summary>
        ///     Turn input echoing back on after turning it off
        /// </summary>
        /// <see cref="echoOff"/>
        public void echoOn()
        {
            writeToBuffer("\x1B[0m");
        }

        /// <summary>
        ///     Turn input echoing off, used when accepting passwords
        /// </summary>
        public void echoOff()
        {
            writeToBuffer("\x1B[0;30;40m");
        }

        /// <summary>
        /// Will cause the calling descriptor to snoop the given
        /// descriptor.  All text written to that descriptor will 
        /// also be be written to the caller until unsnoop is called.
        /// </summary>
        /// <param name="snoopee">the descriptor being snooped</param>
        public void snoop(Descriptor snoopee) {
            snoopee.snoopers.Add(this);
        }

        /// <summary>
        /// The caller will stop snooping the given descriptor.
        /// </summary>
        /// <param name="snoopee">the descriptor being unsnooped</param>
        public void unsnoop(Descriptor snoopee) {
            snoopee.snoopers.Remove(this);
        }

        /// <summary>
        ///     Closes the underlying connection
        /// </summary>
        public void close()
        {
            processOutputBuffer();
            reader.Close();
            writer.Close();
            client.Close();
        }
    }
}

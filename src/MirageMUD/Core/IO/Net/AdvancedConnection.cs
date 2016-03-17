using System;
using System.IO;
using System.Net.Sockets;
using Mirage.Core.Collections;
using System.Collections.Concurrent;

namespace Mirage.Core.IO.Net
{
    /// <summary>
    /// transmit type
    /// </summary>
    public enum AdvancedMessageBodyType
    {
        /// <summary>
        /// a simple string message
        /// </summary>
        StringMessage,
        /// <summary>
        /// A json encoded message
        /// </summary>
        JsonEncodedMessage
    }

    /// <summary>
    /// A message for the advanced connection
    /// </summary>
    public class AdvancedMessage
    {
        public AdvancedMessageBodyType BodyType { get; set; }
        public string Name { get; set; }
        public object Body { get; set; }
    }

    /// <summary>
    /// A connection type that is capable of sending/receiving serialized
    /// messages as well as text
    /// </summary>
    public class AdvancedConnection : SocketConnection, IConnection
    {
        protected BinaryReader reader;
        protected BinaryWriter writer;

        protected ConcurrentQueue<AdvancedMessage> inputQueue;

        /// <summary>
        ///     The lines that are waiting to be written to the socket
        /// </summary>
        protected ConcurrentQueue<AdvancedMessage> outputQueue;

        public AdvancedConnection(TcpClient client)
            : base(client)
        {
            inputQueue = new ConcurrentQueue<AdvancedMessage>();
            outputQueue = new ConcurrentQueue<AdvancedMessage>();
            NetworkStream stm = client.GetStream();
            reader = new BinaryReader(stm);
            writer = new BinaryWriter(stm);
        }

        /// <summary>
        ///     Read from the descriptor.  Returns True if successful.
        ///     Populates an internal buffer, which can be read by read_from_buffer.
        /// </summary>
        public override void ReadInput()
        {
            int type = reader.ReadInt32();
            AdvancedMessage msg = new AdvancedMessage();
            msg.BodyType = (AdvancedMessageBodyType)type;
            switch ((AdvancedMessageBodyType)type)
            {
                case AdvancedMessageBodyType.StringMessage:
                    msg.Body = reader.ReadString();
                    break;
                case AdvancedMessageBodyType.JsonEncodedMessage:
                    msg.Name = reader.ReadString();
                    msg.Body = reader.ReadString();
                    break;
                default:
                    throw new Exception("Unrecognized message type: " + type);
            }
            inputQueue.Enqueue(msg);
        }


        public override void FlushOutput()
        {
            bool bProcess = false;
            AdvancedMessage advMsg;
            while (outputQueue.TryDequeue(out advMsg))
            {
                writer.Write((int)advMsg.BodyType);
                writer.Write(advMsg.Name);
                writer.Write((string)advMsg.Body);
                bProcess = true;
            }
            if (bProcess)
            {
                writer.Flush();
            }
        }

        public bool TryGetInput(out AdvancedMessage input)
        {
            input = null;
            if (_closed == 0 && inputQueue.TryDequeue(out input))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void Write(AdvancedMessage message)
        {
            outputQueue.Enqueue(message);
        }

        public override void Dispose()
        {
            base.Dispose();
            ((IDisposable)reader).Dispose();
            ((IDisposable)writer).Dispose();
        }

    }
}

using System;
using System.IO;
using System.Net.Sockets;
using Mirage.Core.Collections;

namespace Mirage.IO.Net
{
    public enum AdvancedClientTransmitType
    {
        StringMessage,
        JsonEncodedMessage
    }

    public class AdvancedMessage
    {
        public AdvancedClientTransmitType type;
        public string name;
        public object data;
    }

    public class AdvancedConnection : SocketConnection, IConnection
    {
        protected BinaryReader reader;
        protected BinaryWriter writer;

        protected ISynchronizedQueue<AdvancedMessage> inputQueue;

        /// <summary>
        ///     The lines that are waiting to be written to the socket
        /// </summary>
        protected ISynchronizedQueue<AdvancedMessage> outputQueue;

        public AdvancedConnection(TcpClient client)
            : base(client)
        {
            inputQueue = new SynchronizedQueue<AdvancedMessage>();
            outputQueue = new SynchronizedQueue<AdvancedMessage>();
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
            msg.type = (AdvancedClientTransmitType)type;
            switch ((AdvancedClientTransmitType)type)
            {
                case AdvancedClientTransmitType.StringMessage:
                    msg.data = reader.ReadString();
                    break;
                case AdvancedClientTransmitType.JsonEncodedMessage:
                    msg.name = reader.ReadString();
                    msg.data = reader.ReadString();
                    break;
                default:
                    throw new Exception("Unrecognized message type: " + type);
            }
            inputQueue.Enqueue(msg);
        }


        public override void FlushOutput()
        {
            bool bProcess = false;
            while (outputQueue.Count > 0)
            {
                AdvancedMessage advMsg = outputQueue.Dequeue();
                writer.Write((int)advMsg.type);
                writer.Write(advMsg.name);
                writer.Write((string)advMsg.data);
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

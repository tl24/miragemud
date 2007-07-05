using System;
using System.Collections.Generic;
using System.Text;
using Mirage.Data;
using Mirage.Util;
using System.Net.Sockets;
using System.IO;
using JsonExSerializer;
using Mirage.Communication;
using Mirage.Command;

namespace Mirage.IO
{
    public enum AdvancedClientTransmitType
    {
        StringMessage,
        JsonEncodedMessage
    }

    /// <summary>
    /// Mud client class that can accept and send objects and other complex types
    /// more advanced than strings
    /// </summary>
    public class AdvancedClient : ClientBase
    {
        protected BinaryReader reader;
        protected BinaryWriter writer;

        protected Queue<AdvancedMessage> inputQueue;

        public AdvancedClient()
        {
            inputQueue = new Queue<AdvancedMessage>();
        }

        public override void Open(TcpClient client)
        {
            base.Open(client);
            NetworkStream stm = client.GetStream();
            reader = new BinaryReader(stm);
            writer = new BinaryWriter(stm);
        }
        public override void ProcessInput()
        {
            if (ReadClient())
            {
                AdvancedMessage msg = inputQueue.Dequeue();
                if (msg != null)
                {
                    if (msg.type == AdvancedClientTransmitType.JsonEncodedMessage)
                    {
                        Serializer serializer = Serializer.GetSerializer(typeof(object));
                        msg.data = serializer.Deserialize((string)msg.data);
                    }
                    if (msg.type == AdvancedClientTransmitType.StringMessage)
                    {
                        if (StateHandler != null)
                        {
                            StateHandler.HandleInput((string) msg.data);
                        }
                        else
                        {
                            Interpreter.executeCommand(Player, (string)msg.data);
                        }                        
                    }
                }
            }
        }

        /// <summary>
        ///     Read from the descriptor.  Returns True if successful.
        ///     Populates an internal buffer, which can be read by read_from_buffer.
        /// </summary>
        private bool ReadClient()
        {
            int available = _client.Available;

            int type = reader.ReadInt32();
            AdvancedMessage msg = new AdvancedMessage();
            msg.type = (AdvancedClientTransmitType) type;
            switch ((AdvancedClientTransmitType) type)
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
            CommandRead = true;
            inputQueue.Enqueue(msg);
            return true;
        }

        public override void FlushOutput()
        {
            bool bProcess = false;
            while (outputQueue.Count > 0)
            {
                Message msg = outputQueue.Dequeue();
                if (msg is ResourceMessage)
                {
                    msg = new StringMessage(msg.MessageType, msg.Name, msg.ToString());
                }
                AdvancedMessage advMsg = new AdvancedMessage();
                advMsg.type = AdvancedClientTransmitType.JsonEncodedMessage;
                advMsg.name = msg.Name;
                Serializer serializer = Serializer.GetSerializer(typeof(object));
                advMsg.data = serializer.Serialize(msg);
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

        protected class AdvancedMessage
        {
            public AdvancedClientTransmitType type;
            public string name;
            public object data;
        }
    }
}
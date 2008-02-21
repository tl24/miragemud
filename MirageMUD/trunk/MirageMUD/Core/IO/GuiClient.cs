using System;
using System.Collections.Generic;
using System.Text;
using Mirage.Core.Data;
using Mirage.Core.Util;
using System.Net.Sockets;
using System.IO;
using JsonExSerializer;
using Mirage.Core.Communication;
using Mirage.Core.Command;

namespace Mirage.Core.IO
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
    public class GuiClient : ClientBase
    {
        protected BinaryReader reader;
        protected BinaryWriter writer;

        protected ISynchronizedQueue<AdvancedMessage> inputQueue;

        /// <summary>
        ///     The lines that are waiting to be written to the socket
        /// </summary>
        protected ISynchronizedQueue<AdvancedMessage> outputQueue;


        public GuiClient(TcpClient client) : base(client)
        {
            inputQueue = new SynchronizedQueue<AdvancedMessage>();
            outputQueue = new SynchronizedQueue<AdvancedMessage>();
            NetworkStream stm = client.GetStream();
            reader = new BinaryReader(stm);
            writer = new BinaryWriter(stm);
        }

        public override void ProcessInput()
        {
            AdvancedMessage msg = null;
            if (_closed == 0 && inputQueue.TryDequeue(out msg))
            {
                CommandRead = true;
                if (msg.type == AdvancedClientTransmitType.JsonEncodedMessage)
                {
                    Serializer serializer = Serializer.GetSerializer(typeof(object));
                    serializer.Context.ReferenceWritingType = SerializationContext.ReferenceOption.WriteIdentifier;
                    msg.data = serializer.Deserialize((string)msg.data);
                }
                if (msg.type == AdvancedClientTransmitType.StringMessage)
                {
                    if (LoginHandler != null)
                    {
                        LoginHandler.HandleInput((string)msg.data);
                    }
                    else if (((string)msg.data).Trim().Length > 0)
                    {
                        Interpreter.ExecuteCommand(Player, (string)msg.data);
                    }
                }
                else
                {
                    if (LoginHandler != null)
                    {
                        LoginHandler.HandleInput(msg.data);
                    }
                    else
                    {
                        MethodInvoker.Interpret(this.Player, msg.name, (object[])msg.data);
                    }
                }
            }
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

        /// <summary>
        /// Write the specified text to the descriptors output buffer. 
        /// </summary>
        public override void Write(IMessage message)
        {
            if (_closed == 0)
            {
                if (!message.CanTransferMessage)
                {
                    message = message.GetTransferMessage();
                }
                AdvancedMessage advMsg = new AdvancedMessage();
                advMsg.type = AdvancedClientTransmitType.JsonEncodedMessage;
                advMsg.name = message.QualifiedName.ToString();
                Serializer serializer = Serializer.GetSerializer(typeof(object));
                serializer.Context.ReferenceWritingType = SerializationContext.ReferenceOption.WriteIdentifier;
                advMsg.data = serializer.Serialize(message);

                outputQueue.Enqueue(advMsg);
                OutputWritten = true;
            }
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

        public override void Dispose()
        {
            base.Dispose();
            ((IDisposable)reader).Dispose();
            ((IDisposable)writer).Dispose();
        }

        protected class AdvancedMessage
        {
            public AdvancedClientTransmitType type;
            public string name;
            public object data;
        }
    }
}

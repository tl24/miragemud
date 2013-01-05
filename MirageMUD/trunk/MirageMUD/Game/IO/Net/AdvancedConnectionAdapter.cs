using JsonExSerializer;
using Mirage.Game.Command;
using Mirage.Game.Communication;
using Mirage.IO.Net;
using Mirage.Game.Command.Infrastructure;
using Mirage.Core.Messaging;

namespace Mirage.Game.IO.Net
{
    public class AdvancedConnectionAdapter : ConnectionAdapterBase, IConnectionAdapter<AdvancedConnection>
    {
        AdvancedConnection _connection;

        public AdvancedConnectionAdapter(AdvancedConnection connection)
            : base(connection)
        {
            _connection = connection;
        }

        public override void ProcessInput()
        {
            AdvancedMessage msg = null;
            if (_connection.TryGetInput(out msg))
            {
                CommandRead = true;
                if (msg.type == AdvancedClientTransmitType.JsonEncodedMessage)
                {
                    Serializer serializer = new Serializer(typeof(object));
                    serializer.Config.ReferenceWritingType = SerializationContext.ReferenceOption.WriteIdentifier;
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
        /// Write the specified text to the descriptors output buffer. 
        /// </summary>
        public override void Write(IMessage message)
        {
            if (_connection.IsOpen)
            {
                if (!message.CanTransferMessage)
                {
                    message = message.GetTransferMessage();
                }
                AdvancedMessage advMsg = new AdvancedMessage();
                advMsg.type = AdvancedClientTransmitType.JsonEncodedMessage;
                advMsg.name = message.Name.FullName;
                Serializer serializer = new Serializer(typeof(object));
                serializer.Config.ReferenceWritingType = SerializationContext.ReferenceOption.WriteIdentifier;
                advMsg.data = serializer.Serialize(message);

                _connection.Write(advMsg);
                OutputWritten = true;
            }
        }

    }
}

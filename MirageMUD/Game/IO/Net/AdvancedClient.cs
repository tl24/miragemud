using JsonExSerializer;
using Mirage.Game.Command;
using Mirage.Game.Communication;
using Mirage.Core.IO.Net;
using Mirage.Game.Command;
using Mirage.Core.Messaging;
using Mirage.Core.Command;

namespace Mirage.Game.IO.Net
{
    /// <summary>
    /// Handles communication with an Advanced non-text connection.  The
    /// Advanced client is capable of sending/recieving serialized messages
    /// as well as text and is used for the Area builder.
    /// </summary>
    public class AdvancedClient : ClientBase<ClientPlayerState>
    {
        AdvancedConnection _connection;

        public AdvancedClient(AdvancedConnection connection)
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
                if (msg.BodyType == AdvancedMessageBodyType.JsonEncodedMessage)
                {
                    Serializer serializer = new Serializer(typeof(object));
                    serializer.Config.ReferenceWritingType = SerializationContext.ReferenceOption.WriteIdentifier;
                    msg.Body = serializer.Deserialize((string)msg.Body);
                }
                if (msg.BodyType == AdvancedMessageBodyType.StringMessage)
                {
                    if (ClientState.LoginHandler != null)
                    {
                        ClientState.LoginHandler.HandleInput((string)msg.Body);
                    }
                    else if (((string)msg.Body).Trim().Length > 0)
                    {
                        Interpreter.ExecuteCommand(ClientState.Player, (string)msg.Body);
                    }
                }
                else
                {
                    if (ClientState.LoginHandler != null)
                    {
                        ClientState.LoginHandler.HandleInput(msg.Body);
                    }
                    else
                    {
                        CommandInvoker.Instance.Interpret(ClientState.Player, msg.Name, (object[])msg.Body);
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
                advMsg.BodyType = AdvancedMessageBodyType.JsonEncodedMessage;
                advMsg.Name = message.Name.FullName;
                Serializer serializer = new Serializer(typeof(object));
                serializer.Config.ReferenceWritingType = SerializationContext.ReferenceOption.WriteIdentifier;
                advMsg.Body = serializer.Serialize(message);

                _connection.Write(advMsg);
                OutputWritten = true;
            }
        }

    }
}

using Mirage.Game.Command;
using Mirage.Game.Communication;
using Mirage.IO.Net;
using Mirage.Game.Command.Infrastructure;
using Mirage.Core.Messaging;

namespace Mirage.Game.IO.Net
{
    /// <summary>
    /// Handles communication with a text connection
    /// </summary>
    public class TextClient : ClientBase
    {
        TextConnection _connection;

        public TextClient(TextConnection connection) : base(connection)
        {
            _connection = connection;
        }

        public override void ProcessInput()
        {
            string input;
            if (_connection.TryGetInput(out input))
            {
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
        /// Write the specified text to the descriptors output buffer. 
        /// </summary>
        public override void Write(IMessage message)
        {
            if (_connection.IsOpen)
            {
                _connection.Write(message.Render());
                OutputWritten = true;
            }
        }
    }
}

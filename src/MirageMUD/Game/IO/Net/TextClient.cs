using Mirage.Game.Command;
using Mirage.Game.Communication;
using Mirage.Core.IO.Net;
using Mirage.Game.Command;
using Mirage.Core.Messaging;

namespace Mirage.Game.IO.Net
{
    /// <summary>
    /// Handles communication with a text connection
    /// </summary>
    public class TextClient : TextClientBase<ClientPlayerState>
    {
        TextConnection _connection;

        public TextClient(TextConnection connection) : base(connection)
        {
            _connection = connection;
        }

        protected override void OnInputReceived(string input)
        {
            if (ClientState.LoginHandler != null)
            {
                ClientState.LoginHandler.HandleInput(input);
            }
            else if (input.Trim().Length > 0)
            {
                Interpreter.ExecuteCommand(ClientState.Player, input);
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

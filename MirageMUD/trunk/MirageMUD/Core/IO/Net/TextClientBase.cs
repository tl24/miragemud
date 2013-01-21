using Mirage.Core.IO.Net;
using Mirage.Core.Messaging;

namespace Mirage.Core.IO.Net
{
    /// <summary>
    /// Handles communication with a text connection
    /// </summary>
    public abstract class TextClientBase<TClientState> : ClientBase<TClientState> where TClientState : new()
    {
        TextConnection _connection;

        public TextClientBase(TextConnection connection)
            : base(connection)
        {
            _connection = connection;
        }

        public override void ProcessInput()
        {
            string input;
            if (_connection.TryGetInput(out input))
            {
                CommandRead = true;
                OnInputReceived(input);
            }
        }

        protected abstract void OnInputReceived(string input);

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

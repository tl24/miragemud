using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mirage.Core.Command;
using Mirage.Core.Communication;

namespace Mirage.Core.IO
{
    public class TextConnectionAdapter : ConnectionAdapterBase, IConnectionAdapter<TextConnection>
    {
        TextConnection _connection;

        public TextConnectionAdapter(TextConnection connection) : base(connection)
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

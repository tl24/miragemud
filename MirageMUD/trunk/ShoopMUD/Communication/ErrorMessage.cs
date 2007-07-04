using System;
using System.Collections.Generic;
using System.Text;

namespace Shoop.Communication
{
    public class ErrorMessage : StringMessage
    {
        public ErrorMessage(string name, string message)
            : this(MessageType.PlayerError, name, message)
        {
        }

        public ErrorMessage(MessageType messageType, string name, string message)
            : base(messageType, name, message)
        {
        }
    }
}

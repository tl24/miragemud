using System;
using System.Collections.Generic;
using System.Text;

namespace Mirage.Communication
{

    /// <summary>
    /// Constructs an error message from a resource template
    /// </summary>
    public class ErrorResourceMessage : ResourceMessage
    {
        public ErrorResourceMessage(string messageName, string resourceName)
            :
            this(MessageType.PlayerError, messageName, resourceName)
        {
        }

        public ErrorResourceMessage(MessageType messageType, string messageName, string resourceName)
            :
            base(messageType, messageName, resourceName)
        {
        }

    }
}

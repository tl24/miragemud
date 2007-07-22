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
        public ErrorResourceMessage(string messageName)
            :
            this(MessageType.PlayerError, Namespaces.CommonError, messageName)
        {
        }

        public ErrorResourceMessage(MessageType messageType, Uri Namespace, string messageName)
            :
            base(messageType, Namespace, messageName)
        {
        }

        public ErrorResourceMessage()
        {
        }
    }
}

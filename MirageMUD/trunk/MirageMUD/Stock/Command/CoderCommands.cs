using System;
using System.Collections.Generic;
using System.Text;
using Mirage.Core.Command;
using Mirage.Core.Communication;

namespace Mirage.Stock.Command
{
    [CommandDefaults(Roles="coder")]
    public class CoderCommands
    {

        [CommandAttribute(Description="Clears the cache for the message factory")]
        public static IMessage ClearMessageCache()
        {
            MessageFactory.Clear();
            //don't load a message through the factory since it will reload the namespace
            return new StringMessage(MessageType.Confirmation, "ClearMessageCache", "Message factory cache cleared.");
        }
    }
}

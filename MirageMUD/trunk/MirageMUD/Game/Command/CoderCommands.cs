using System;
using System.Collections.Generic;
using System.Text;
using Mirage.Game.Command;
using Mirage.Game.Communication;

namespace Mirage.Game.Command
{
    [CommandDefaults(Roles="coder")]
    public class CoderCommands
    {
        private IMessageFactory _messageFactory;
        public IMessageFactory MessageFactory
        {
            get { return _messageFactory; }
            set { _messageFactory = value; }
        }

        [CommandAttribute(Description="Clears the cache for the message factory")]
        public IMessage ClearMessageCache()
        {
            MessageFactory.Clear();
            //don't load a message through the factory since it will reload the namespace
            return new StringMessage(MessageType.Confirmation, "ClearMessageCache", "Message factory cache cleared.\r\n");
        }
    }
}

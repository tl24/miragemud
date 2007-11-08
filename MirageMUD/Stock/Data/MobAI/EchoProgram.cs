using System;
using System.Collections.Generic;
using System.Text;
using Mirage.Core.Communication;

namespace Mirage.Stock.Data.MobAI
{
    /// <summary>
    /// Mob program that echo's speech in the room
    /// </summary>
    public class EchoProgram : AIProgram
    {
        public EchoProgram(Mobile mob)
            : base(mob)
        {
        }

        public override AIMessageResult HandleMessage(Mirage.Core.Communication.IMessage message)
        {
            if (message.IsMatch(MessageType.Communication, Namespaces.Communication, "say.others"))
            {
                ResourceMessage msg = (ResourceMessage)message;
                this.Mob.Commands.Enqueue(new MobileStringCommand("say '" + msg["player"] + " said \"" + msg["message"] + "\""));
                return AIMessageResult.MessageHandledContinue;
            }
            return AIMessageResult.MessageNotHandled;
        }
    }
}

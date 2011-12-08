using System;
using System.Collections.Generic;
using System.Text;
using Mirage.Game.Communication;

namespace Mirage.Game.World.MobAI
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

        public override AIMessageResult HandleMessage(Mirage.Game.Communication.IMessage message)
        {
            if (message.IsMatch(Namespaces.Communication, "SayOthers"))
            {
                ResourceMessage msg = (ResourceMessage)message;
                this.Mob.Commands.Enqueue(new MobileStringCommand("say '" + msg["player"] + " said \"" + msg["message"] + "\""));
                return AIMessageResult.MessageHandledContinue;
            }
            return AIMessageResult.MessageNotHandled;
        }
    }
}

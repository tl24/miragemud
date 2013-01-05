using Mirage.Game.Communication;
using Mirage.Game.Command;

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
            if (message.IsMatch(CommunicationCommands.Messages.SayOthers))
            {
                this.Mob.Commands.Enqueue(new MobileStringCommand("say '" + message["actor"] + " said \"" + message["message"] + "\""));
                return AIMessageResult.MessageHandledContinue;
            }
            return AIMessageResult.MessageNotHandled;
        }
    }
}

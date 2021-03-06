using System;
using Mirage.Game.Communication;
using Mirage.Game.Command;

namespace Mirage.Game.World.MobAI
{
    public class Wanderer : AIProgram
    {
        private DateTime lastTime;
        private Random rand;
        private bool processCommand = true;
        public Wanderer(Mobile mob)
            : base(mob)
        {
            rand = new Random((int) DateTime.Now.Ticks);
        }

        public override void GenerateInput()
        {
            //TODO: We need some notification that we actually executed the last move before
            // we try and move again
            if (lastTime.AddSeconds(6) < DateTime.Now && processCommand)
            {
                lastTime = DateTime.Now;

                Room room = Mob.Room;
                if (room == null)
                    return;

                int numRooms = room.Exits.Count;
                int result = rand.Next(numRooms+1); // +1 for small chance of failure
                int i = 0;
                foreach (RoomExit exit in room.Exits.Values)
                {
                    if (i == result)
                    {
                        Mob.Commands.Enqueue(new MobileStringCommand(exit.Direction.ToString()));
                        processCommand = false;
                        break;
                    }
                    i++;
                }
            }
        }

        public override AIMessageResult HandleMessage(Mirage.Core.Messaging.IMessage message)
        {
            if (message.IsMatch(MovementCommands.Messages.GoSelf))
            {
                processCommand = true;
                return AIMessageResult.MessageHandledContinue;
            }
            return AIMessageResult.MessageNotHandled;
        }
    }
}

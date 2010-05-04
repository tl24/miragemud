using System;
using System.Collections.Generic;
using System.Text;
using Mirage.Core.Communication;

namespace Mirage.Stock.Data.MobAI
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

                Room room = Mob.Container as Room;
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

        public override AIMessageResult HandleMessage(Mirage.Core.Communication.IMessage message)
        {
            if (message.IsMatch(Namespaces.Movement, "YouGoDirection"))
            {
                processCommand = true;
                return AIMessageResult.MessageHandledContinue;
            }
            return AIMessageResult.MessageNotHandled;
        }
    }
}

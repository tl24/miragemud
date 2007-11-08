using System;
using System.Collections.Generic;
using System.Text;

namespace Mirage.Stock.Data.MobAI
{
    public class Wanderer : AIProgram
    {
        private DateTime lastTime;
        private Random rand;

        public Wanderer(Mobile mob)
            : base(mob)
        {
            rand = new Random((int) DateTime.Now.Ticks);
        }

        public override void GenerateInput()
        {
            if (lastTime.AddSeconds(6) < DateTime.Now)
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
                        break;
                    }
                    i++;
                }
            }
        }
    }
}

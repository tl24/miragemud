using System.Collections.Generic;
using System.Linq;

namespace Mirage.Game.World
{
    public class MobTemplate : LivingTemplateBase
    {
        public MobTemplate()
        {
            Mobiles = new LinkedList<Mobile>();
        }

        /// <summary>
        /// The list of mobs created from this template
        /// </summary>
        public ICollection<Mobile> Mobiles { get; private set; }

        /// <summary>
        /// Maximum number of this mobile allowed to be created in a room at one time
        /// </summary>
        public int MaxRoomCount { get; set; }

        /// <summary>
        /// Maximum total number of this mobile allowed
        /// </summary>
        public int MaxCount { get; set; }

        public List<MobReset> Resets { get; set; }

        /// <summary>
        /// Creates a new mobile from this template and returns it.  No checks for
        /// max counts are done.
        /// </summary>
        /// <returns>the newly created mobile</returns>
        public Mobile Create()
        {
            Mobile mob = new Mobile(this);
            this.CopyTo(mob);
            Mobiles.Add(mob);
            return mob;
        }

        public void ProcessResets()
        {
            foreach(MobReset reset in Resets) {
                if (MaxCount != 0 && Mobiles.Count >= MaxCount)
                    return;

                Room room = reset.GetRoom();
                if (MaxRoomCount != 0 && GetMobRoomCount(room) >= MaxRoomCount)
                    continue;

                Mobile mob = Create();
                room.Add(mob);
            }
        }

        private int GetMobRoomCount(Room room)
        {
            int count = 0;
            foreach (Mobile mob in room.OfType<Mobile>())
            {
                if (mob.Template == this)
                    count++;
            }
            return count;
        }

        [EditorParent(2)]
        public Area Area { get; set; }
    }
}

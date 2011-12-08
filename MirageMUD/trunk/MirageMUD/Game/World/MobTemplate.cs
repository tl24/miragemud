using System;
using System.Collections.Generic;
using System.Text;
using Mirage.Game.World;
using JsonExSerializer;

namespace Mirage.Game.World
{
    public class MobTemplate : LivingTemplateBase
    {
        private LinkedList<Mobile> _mobiles;
        private int _maxRoomCount;
        private int _maxCount;
        private List<MobReset> _resets;
        private Area _area;

        public MobTemplate()
        {
            _mobiles = new LinkedList<Mobile>();
        }

        public ICollection<Mobile> Mobiles
        {
            get { return this._mobiles; }
        }

        /// <summary>
        /// Maximum number of this mobile allowed to be created in a room at one time
        /// </summary>
        public int MaxRoomCount
        {
            get { return this._maxRoomCount; }
            set { this._maxRoomCount = value; }
        }

        /// <summary>
        /// Maximum total number of this mobile allowed
        /// </summary>
        public int MaxCount
        {
            get { return this._maxCount; }
            set { this._maxCount = value; }
        }

        public List<MobReset> Resets
        {
            get { return this._resets; }
            set { this._resets = value; }
        }

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
            foreach (Mobile mob in room.Contents(typeof(Mobile)))
            {
                if (mob.Template == this)
                    count++;
            }
            return count;
        }

        [EditorParent(2)]
        public Area Area
        {
            get { return this._area; }
            set { this._area = value; }
        }
    }
}

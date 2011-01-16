using System;
using System.Collections.Generic;
using System.Text;

namespace Mirage.Core.Data.Items
{
    [Flags]
    public enum WearLocations
    {
        Head = 0x1,
        Neck = 0x2,
        Torso = 0x4,
        Arms = 0x8,
        LeftWrist = 0x10,
        RightWrist = 0x20,
        Hands = 0x40,
        Waist = 0x80,
        Legs = 0x100,
        LeftAnkle = 0x200,
        RightAnkle = 0x400,
        Feet = 0x800
    }

    public class Armor : ItemBase
    {
        private WearLocations _wearFlags;
        private Living _wearer;

        public WearLocations WearFlags
        {
            get { return this._wearFlags; }
            set { this._wearFlags = value; }
        }

        public Living Wearer
        {
            get { return this._wearer; }
            set { this._wearer = value; }
        }        
    }
}

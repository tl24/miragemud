using System;
using System.Collections.Generic;
using System.Text;
using Mirage.Game.World;

namespace Mirage.Game.World
{
    /// <summary>
    /// Base class for Mob Templates as well as Living, since many of the
    /// properties are shared
    /// </summary>
    public class LivingTemplateBase : ViewableBase
    {
        private int _level;
        private GenderType _gender;

        public LivingTemplateBase()
        {
            _level = 1;
            _gender = GenderType.Other;
        }

        public override void CopyTo(BaseData other)
        {
            base.CopyTo(other);
            LivingTemplateBase otherLiving = other as LivingTemplateBase;
            if (otherLiving != null)
            {
                otherLiving.Level = this.Level;
                otherLiving.Gender = this.Gender;
            }
        }
        
        /// <summary>
        ///     The player or mobile's level
        /// </summary>

        public int Level
        {
            get { return _level; }
            set { _level = value; }
        }

        /// <summary>
        ///     The object's gender
        /// </summary>
        public GenderType Gender
        {
            get { return _gender; }
            set { _gender = value; }
        }
    }
}

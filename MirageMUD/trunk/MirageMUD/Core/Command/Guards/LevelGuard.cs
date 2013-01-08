using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mirage.Core.Command.Guards
{
    public class LevelGuard : ICommandGuard
    {
        public LevelGuard(int level)
        {
            if (level < 1)
                throw new ArgumentOutOfRangeException("level", level, "Level must be greater than 0");
            this.Level = level;
        }

        public int Level { get; private set; }



        #region ICommandGuard Members

        public bool IsSatisified(IActor actor)
        {
            return (actor.Level >= Level);
        }

        #endregion
    }
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mirage.Game.Command.Infrastructure.Guards;

namespace Mirage.Game.Command
{
    /// <summary>
    /// Restricts a command to a player at or above the required level
    /// </summary>
    [AttributeUsageAttribute(System.AttributeTargets.Class | System.AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    public class LevelRestrictionAttribute : CommandRestrictionAttribute
    {
        public LevelRestrictionAttribute(int level)
        {
            this.Level = level;
        }

        public int Level { get; private set; }

        public override Infrastructure.Guards.ICommandGuard CreateGuard()
        {
            return new LevelGuard(Level);
        }
    }
}

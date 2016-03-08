using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mirage.Game.World
{
    public interface ICombatModule
    {
        void ProcessCombat();
        int CombatPulseMultiplier { get; set; }
    }
}

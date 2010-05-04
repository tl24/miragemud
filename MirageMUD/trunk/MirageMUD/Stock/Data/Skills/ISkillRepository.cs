﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mirage.Stock.Data.Skills
{
    public interface ISkillRepository
    {
        ICollection<SkillDefinition> SkillDefinitions { get; }
        
        ICollection<ITrainingBonus> TrainingBonuses { get; }

        IPlayerAvailableSkills GetAvailableSkillsForPlayer(Player player);
    }
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mirage.Core.Data.Skills
{
    public class SkillRepository : ISkillRepository
    {
        List<SkillDefinition> skillDefinitions = new List<SkillDefinition>();
        List<ITrainingBonus> trainingBonuses = new List<ITrainingBonus>();

        public ICollection<SkillDefinition> SkillDefinitions
        {
            get { return skillDefinitions; }
        }

        public ICollection<ITrainingBonus> TrainingBonuses
        {
            get { return trainingBonuses; }
        }

        public IPlayerAvailableSkills GetAvailableSkillsForPlayer(Player player)
        {
            AvailableSkillsLoader loader = new AvailableSkillsLoader(player.Skills.Values, SkillDefinitions, TrainingBonuses);
            loader.GenerateSkillList();
            return loader;
        }

        
    }
}
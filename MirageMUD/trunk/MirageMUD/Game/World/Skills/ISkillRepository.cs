using System.Collections.Generic;

namespace Mirage.Game.World.Skills
{
    public interface ISkillRepository
    {
        ICollection<SkillDefinition> SkillDefinitions { get; }
        
        ICollection<ITrainingBonus> TrainingBonuses { get; }

        IPlayerAvailableSkills GetAvailableSkillsForPlayer(Player player);
    }
}

using System.Collections.Generic;
namespace Mirage.Game.World.Skills
{
    public interface IPlayerAvailableSkills
    {
        IEnumerable<AvailableSkill> AvailableSkills { get; }
        IEnumerable<AvailableSkill> UnavailableSkills { get; }
    }
}

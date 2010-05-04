using System;
using System.Collections.Generic;
namespace Mirage.Stock.Data.Skills
{
    public interface IPlayerAvailableSkills
    {
        IEnumerable<AvailableSkill> AvailableSkills { get; }
        IEnumerable<AvailableSkill> UnavailableSkills { get; }
    }
}

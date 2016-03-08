using System;
using System.Collections.Generic;
using System.Linq;

namespace Mirage.Game.World.Skills
{
    public class Proficiency
    {
        public static readonly Proficiency AnyProficiency = new Proficiency("any", 0);

        public Proficiency(string name, int minLevel)
        {
            this.Name = name;
            this.MinLevel = minLevel;
        }

        public string Name { get; set; }
        public int MinLevel { get; set; }

        public bool IsSatisified(Skill skill)
        {
            return (skill.Level >= MinLevel);
        }
    }
    public class Skill
    {
        public Skill(SkillDefinition definition, int level)
        {
            this.Definition = definition;
            this.Level = level;
        }

        public Skill(SkillDefinition definition)
            : this(definition, 0)
        {
        }

        public SkillDefinition Definition { get; private set; }
        public int Level { get; set; }
    }

    public class SkillDefinition
    {
        private List<ISkillRequirement> requirements;

        public SkillDefinition()
        {
        }

        public SkillDefinition(string name, int cost)
        {
            this.Name = name;
            this.Cost = cost;
        }

        public string Name { get; set; }
        public int Cost { get; set; }

        public bool RequirementsSatisfied(IEnumerable<Skill> skills)
        {
            if (requirements == null)
                return true;
            return requirements.All((req) => (req.IsSatisfied(skills)));
        }

        public void AddRequirement(PreReqSkillRequirement preReqSkillRequirement)
        {
            if (requirements == null)
                requirements = new List<ISkillRequirement>();
            requirements.Add(preReqSkillRequirement);
        }

        public void RequiresSkill(SkillDefinition skill)
        {
            AddRequirement(new PreReqSkillRequirement(skill));
        }

        public void RequiresSkill(SkillDefinition skill, Proficiency atProficiency)
        {
            AddRequirement(new PreReqSkillRequirement(skill, atProficiency));
        }
    }

    public interface ISkillRequirement
    {
        bool IsSatisfied(IEnumerable<Skill> skills);
    }

    public class PreReqSkillRequirement : ISkillRequirement
    {
        public PreReqSkillRequirement(SkillDefinition requiredSkill) : this(requiredSkill, null)
        {
        }

        public PreReqSkillRequirement(SkillDefinition requiredSkill, Proficiency proficiency)
        {
            this.RequiredSkill = requiredSkill;
            this.RequiredProficiency = proficiency ?? Proficiency.AnyProficiency;
        }

        public SkillDefinition RequiredSkill { get; private set; }
        public Proficiency RequiredProficiency { get; set; }

        public bool IsSatisfied(IEnumerable<Skill> skills)
        {
            return skills.Any((sk) => (sk.Definition.Equals(RequiredSkill) && RequiredProficiency.IsSatisified(sk)));
        }
    }

    public class Weapon : SkillDefinition
    {
        public Weapon()
            : base()
        {
        }
        public Weapon(string name, int cost)
            : base(name, cost)
        {
        }
    }

    public class Spell : SkillDefinition
    {
        public Spell()
            : base()
        {
        }
        public Spell(string name, int cost)
            : this(name, cost, 20)
        {
        }

        public Spell(string name, int cost, int manaCost)
            : base(name, cost)
        {
            this.ManaCost = manaCost;
        }

        public int ManaCost { get; set; }
    }

    public interface ITrainingBonus {
        void ApplyBonuses(IEnumerable<AvailableSkill> skills, IEnumerable<Skill> learnedSkills);
    }

    public class SkillFamily : ITrainingBonus
    {
        public SkillFamily(string name)
        {
            this.Name = name;
            this.Members = new List<SkillDefinition>();
        }

        public SkillFamily(string name, float bonus)
        {
            this.Name = name;
            this.Bonus = bonus;
            this.Members = new List<SkillDefinition>();
        }

        public SkillFamily(string name, float bonus, IEnumerable<SkillDefinition> members)
        {
            this.Name = name;
            this.Bonus = bonus;
            this.Members = new List<SkillDefinition>(members);
        }

        public string Name { get; set; }
        public float Bonus { get; set; }
        public IList<SkillDefinition> Members { get; private set; }

        #region ITrainingBonus Members

        public void ApplyBonuses(IEnumerable<AvailableSkill> skills, IEnumerable<Skill> learnedSkills)
        {
            if (Members == null || Members.Count == 0)
                throw new InvalidOperationException("No members defined for SkillFamily " + this.Name);

            if (!skills.Any())
                return;

            var countQuery = from ls in learnedSkills
                    join m in this.Members
                    on ls.Definition equals m
                    select ls;
            int skillCount = countQuery.Count();
            int appliedBonus = (int) Math.Floor(Bonus * skillCount);
            if (appliedBonus == 0)
                return;

            var appliedQuery = from avs in skills
                               join m in this.Members
                               on avs.Skill equals m
                               select avs;
            foreach(AvailableSkill avs in appliedQuery) {
                avs.Cost -= Math.Min(avs.Cost, appliedBonus);
            }
        }

        #endregion
    }

    public class AvailableSkill
    {
        public AvailableSkill(SkillDefinition skillDef)
        {
            this.Skill = skillDef;
            this.Cost = skillDef.Cost;
        }
        public SkillDefinition Skill { get; set; }
        public int Cost { get; set; }

        public bool IsAvailable
        {
            get { return Cost >= 0; }
        }
    }

    public class AvailableSkillsLoader : Mirage.Game.World.Skills.IPlayerAvailableSkills
    {
        private IEnumerable<Skill> learnedSkills;
        private IEnumerable<SkillDefinition> definitions;
        private IEnumerable<ITrainingBonus> bonuses;

        private List<AvailableSkill> availableSkills = new List<AvailableSkill>();

        public AvailableSkillsLoader(IEnumerable<Skill> learnedSkills, IEnumerable<SkillDefinition> skills, IEnumerable<ITrainingBonus> bonuses)
        {
            this.learnedSkills = learnedSkills;
            this.definitions = skills;
            this.bonuses = bonuses;
        }

        public void GenerateSkillList()
        {
            foreach (SkillDefinition definition in definitions)
            {
                if (!learnedSkills.Any((ls) => (ls.Definition.Equals(definition))))
                    availableSkills.Add(new AvailableSkill(definition));
            }

            foreach(AvailableSkill availSkill in availableSkills) {
                if (!availSkill.Skill.RequirementsSatisfied(learnedSkills))
                    availSkill.Cost = -1;
            }

            foreach(ITrainingBonus bonus in bonuses)
                bonus.ApplyBonuses(availableSkills.Where((avs)=>(avs.IsAvailable)), learnedSkills);
        }

        public IEnumerable<AvailableSkill> AvailableSkills
        {
            get { return availableSkills.Where((avs) => (avs.IsAvailable)); }
        }

        public IEnumerable<AvailableSkill> UnavailableSkills
        {
            get { return availableSkills.Where((avs) => (!avs.IsAvailable)); }
        }
    }
}

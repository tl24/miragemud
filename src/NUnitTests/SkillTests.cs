using System.Linq;
using Mirage.Game.World.Skills;
using NUnit.Framework;

namespace NUnitTests
{
    [TestFixture]
    public class SkillTests
    {
        [Test]
        public void RequirementsSatisfied_SucceedsWithNoRequirements()
        {
            SkillDefinition skill = new SkillDefinition("bash", 5);
            Assert.IsTrue(skill.RequirementsSatisfied(Enumerable.Empty<Skill>()));
        }

        [Test]
        public void RequirementsSatisfied_FailsIfPreReqNotLearned()
        {
            SkillDefinition fireball = new SkillDefinition("fireball", 2);
            SkillDefinition firestorm = new SkillDefinition("firestorm", 5);
            firestorm.AddRequirement(new PreReqSkillRequirement(fireball));
            Assert.IsFalse(firestorm.RequirementsSatisfied(Enumerable.Empty<Skill>()));
        }

        [Test]
        public void RequirementsSatisfied_FailsIfPreReqNotProficient()
        {
            SkillDefinition fireball = new SkillDefinition("fireball", 2);
            SkillDefinition firestorm = new SkillDefinition("firestorm", 5);
            firestorm.AddRequirement(new PreReqSkillRequirement(fireball, new Proficiency("average", 50)));

            Assert.IsFalse(firestorm.RequirementsSatisfied(new Skill[] { new Skill(fireball,10) }));
        }

        [Test]
        public void RequirementsSatisfied_SucceedsIfPreReqLearnedAndProficient()
        {
            SkillDefinition fireball = new SkillDefinition("fireball", 2);
            SkillDefinition firestorm = new SkillDefinition("firestorm", 5);
            firestorm.AddRequirement(new PreReqSkillRequirement(fireball, new Proficiency("average", 50)));

            Assert.IsTrue(firestorm.RequirementsSatisfied(new Skill[] { new Skill(fireball, 50) }));
        }

        [Test]
        public void Bonus_WhenLessThanOne_NotApplied()
        {
            SkillFamily family = new SkillFamily("weaponsmaster", .5f);
            SkillDefinition dagger = new SkillDefinition("dagger", 3);
            SkillDefinition sword = new SkillDefinition("sword", 3);
            family.Members.Add(dagger);
            family.Members.Add(sword);
            AvailableSkill swordAvs = new AvailableSkill(sword);

            Skill learnedDagger = new Skill(dagger, 80);
            family.ApplyBonuses(new AvailableSkill[] { swordAvs }, new Skill[] { learnedDagger });

            Assert.AreEqual(3, swordAvs.Cost, "Sword Cost");
        }

        [Test]
        public void Bonus_WhenEnoughLearnedSkills_BonusApplied()
        {
            SkillFamily family = new SkillFamily("weaponsmaster", 1f);
            SkillDefinition dagger = new SkillDefinition("dagger", 3);
            SkillDefinition sword = new SkillDefinition("sword", 3);
            family.Members.Add(dagger);
            family.Members.Add(sword);
            AvailableSkill swordAvs = new AvailableSkill(sword);

            Skill learnedDagger = new Skill(dagger, 80);
            family.ApplyBonuses(new AvailableSkill[] { swordAvs }, new Skill[] { learnedDagger });

            Assert.AreEqual(2, swordAvs.Cost, "Sword Cost");
        }
    }
}

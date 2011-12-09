using System.Linq;

namespace Mirage.Game.World.Skills
{
    public class SkillsLoader : IInitializer
    {
        ISkillRepository skillRepository;
        Proficiency adept = new Proficiency("adept", 30);
        Proficiency proficient = new Proficiency("proficient", 50);
        Proficiency mastered = new Proficiency("mastered", 90);
        Proficiency maxProficiency = new Proficiency("max", 100);

        public SkillsLoader(ISkillRepository skillRepository)
        {
            this.skillRepository = skillRepository;
        }

        public string Name
        {
            get { return "Skills Loader"; }
        }

        public void Execute()
        {
            LoadSkills();
            LoadSkillFamilies();
        }

        private void LoadSkills()
        {
            Weapon sword = Add(new Weapon("sword", 2));
            Weapon dagger = Add(new Weapon("dagger", 2));
            Spell fireball = Add(new Spell("fireball", 2));
            Spell firestorm = Add(new Spell("firestorm", 4));
            firestorm.RequiresSkill(fireball, proficient);
            Spell inferno = Add(new Spell("inferno", 6));
            inferno.RequiresSkill(firestorm, mastered);
        }

        private void LoadSkillFamilies()
        {
            SkillFamily weaponsmaster = new SkillFamily("weaponsmaster", .5f);
            foreach (SkillDefinition w in skillRepository.SkillDefinitions.Where((sk) => (sk is Weapon)))
                weaponsmaster.Members.Add(w);
            skillRepository.TrainingBonuses.Add(weaponsmaster);
        }

        private SkillDefinition Add(SkillDefinition def)
        {
            skillRepository.SkillDefinitions.Add(def);
            return def;
        }

        private T Add<T>(T def) where T : SkillDefinition
        {
            skillRepository.SkillDefinitions.Add(def);
            return def;
        }
    }
}

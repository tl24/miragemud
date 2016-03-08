
namespace Mirage.Game.World
{
    /// <summary>
    /// Base class for Mob Templates as well as Living, since many of the
    /// properties are shared
    /// </summary>
    public abstract class LivingTemplateBase : ViewableBase
    {
        protected LivingTemplateBase()
        {
            Level = 1;
            Gender = GenderType.Other;
        }

        public override void CopyTo(BaseData other)
        {
            base.CopyTo(other);
            LivingTemplateBase otherLiving = other as LivingTemplateBase;
            if (otherLiving != null)
            {
                otherLiving.Level = this.Level;
                otherLiving.Gender = this.Gender;
                otherLiving.HitPoints = this.MaxHitPoints; // start at max
                otherLiving.MaxHitPoints = this.MaxHitPoints;
            }
        }
        
        /// <summary>
        ///     The player or mobile's level
        /// </summary>

        public int Level { get; set; }

        /// <summary>
        ///     The object's gender
        /// </summary>
        public GenderType Gender { get; set; }

        public int HitPoints { get; set; }
        public int MaxHitPoints { get; set; }
    }
}

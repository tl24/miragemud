using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mirage.Game.World
{
    public class DefaultCombatModule : ICombatModule
    {
        private MudWorld _world;
        private uint _pulses;

        public DefaultCombatModule(MudWorld world)
        {
            _world = world;
            _pulses = 0;
            CombatPulseMultiplier = 6;
        }

        public int CombatPulseMultiplier { get; set; }

        public void ProcessCombat()
        {
            if (_pulses++ % CombatPulseMultiplier != 0)
                return;

            foreach (Living living in _world.LivingThings.Where(lv => lv.IsFighting))
            {
                var attacker = living;
                var target = living.Fighting;
                if (target == null)
                    continue;

                int damage = CalculateDamage(attacker, target);
                if (damage == 0)
                {
                    attacker.ToSelf("combat.miss", "You miss ${target}", target);
                    attacker.ToTarget("combat.miss", "${actor} misses you!", target);
                    attacker.ToBystanders("combat.miss", "${actor} misses ${target}!");
                }
                else
                {
                    target.HitPoints -= damage;
                    attacker.ToSelf("combat.damage", "You hit ${target} for ${damage} damage.", target, new { damage });
                    attacker.ToTarget("combat.damage", "${actor} hits you for ${damage} damage!  Current Hp ${hp}/${maxhp}!", target, new { damage, hp = target.HitPoints, maxhp = target.MaxHitPoints });
                    attacker.ToBystanders("combat.damage", "${actor} hits ${target}!");

                    if (target.HitPoints <= 0)
                    {
                        attacker.ToSelf("combat.death", "You have killed ${target}!", target);
                        attacker.ToTarget("combat.damage", "${actor} has killed you!", target);
                        attacker.ToBystanders("combat.damage", "${actor} has killed ${target}!");
                        if (target is Mobile)
                        {
                            // for mobiles, remove
                            target.Room.Remove(target);
                        }
                        else
                        {
                            // reset
                            target.HitPoints = target.MaxHitPoints;
                        }
                        target.Fighting = null;
                        // TODO: look for other people fighting
                        attacker.Fighting = null;
                        attacker.HitPoints = attacker.MaxHitPoints;
                    }
                    else
                    {
                        // make sure we're still fighting
                        if (!target.IsFighting)
                        {
                            target.Fighting = target.Fighting ?? attacker;
                            target.ToSelf("combat.attack", "You are now fighting ${target}!", attacker);
                            target.ToBystanders("combat.attack", "${actor} is now fighting ${target}", attacker);
                        }
                    }
                }
            }
        }

        private int CalculateDamage(Living attacker, Living target)
        {
            return Dice.Default.Roll(0, attacker is Player ? 10 : 5);
        }
    }
}

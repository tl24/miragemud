using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mirage.Core.Command;
using Mirage.Game.World;
using Mirage.Game.World.Attribute;
using Mirage.Core.Messaging;

namespace Mirage.Game.Command
{
    public class CombatCommands : CommandDefaults
    {
        [Command(Description="Start attacking {target}", Aliases=new [] {"kill"})]
        public void fight([Actor] Living actor, [Lookup("Room/LivingThings")] Living target)
        {
            if (actor.IsFighting)
            {
                //TODO: switch targets?
                actor.ToSelf("combat.fight.error.alreadyfighting", "You are already fighting!");
                return;
            }
            actor.Fighting = target;
            if (target.Fighting == null)
            {
                target.Fighting = actor;
            }
            actor.ToSelf("combat.attack", "You attack ${target}", target);
            actor.ToTarget("combat.attack", "${actor} attacks you!", target);
            actor.ToBystanders("combat.attack", "${actor} attacks ${target}", target);
        }

        [Command(Description = "Flee from combat")]
        public void flee([Actor] Living actor)
        {
            if (!actor.IsFighting)
            {
                actor.ToSelf("combat.error.flee.notfighting", "You aren't fighting anyone!");
                return;
            }
            if (actor.Room == null)
            {
                actor.ToSelf(MovementCommands.Messages.NotInRoom);
                return;
            }

            var availableExits = actor.Room.Exits.Values.Where(exit => OpenableAttribute.IsOpen(exit)).ToArray();
            if (!availableExits.Any())
            {
                actor.ToSelf("combat.error.flee.noexit", "There's nowhere to go!");
                return;
            }
            int chance = Dice.Default.Roll(1, 5);
            if (chance > 1)
            {
                var exit = availableExits[Dice.Default.Roll(0, availableExits.Length - 1)];
                actor.ToSelf("combat.flee", "You flee the fight!");
                actor.ToRoom("combat.flee", "${actor} runs away!");
                var target = actor.Fighting;
                actor.Fighting = null;
                foreach(var living in actor.Room.LivingThings) {
                    if (living.Fighting == actor)
                    {
                        living.Fighting = null;
                    }
                }
                CommandInvoker.Instance.Interpret(actor, exit.Direction.ToString());
            }
            else
            {
                actor.ToSelf("combat.error.flee.failed", "You can't get away!");
            }
        }
    }
}

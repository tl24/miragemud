using System.Collections.Generic;
using System.Text;
using Mirage.Game.Communication;
using Mirage.Game.World;
using Mirage.Game.World.Attribute;
using Mirage.Game.World.Query;
using Mirage.IO.Net;
using System.Collections;
using Mirage.Game.Command.Infrastructure;
using Mirage.Core.Messaging;

namespace Mirage.Game.Command
{
    public class MiscCommands : CommandDefaults
    {
        public class Messages
        {
            public static readonly MessageDefinition SayOthers = new MessageDefinition("communication.say.others", "${actor} says '${message}'.");
            public static readonly MessageDefinition SaySelf = new MessageDefinition("communication.say.self", "You said '${message}'.");
            public static readonly MessageDefinition TellOthers = new MessageDefinition("communication.tell.others", "${actor} tells you '${message}'.");
            public static readonly MessageDefinition TellSelf = new MessageDefinition("communication.tell.self", "You tell ${target} '${message}'.");

            public static readonly MessageDefinition BeingIgnored = new MessageDefinition("communication.common.beingignored", "${target} is ignoring you.");
            public static readonly MessageDefinition LookTarget = new MessageDefinition("system.look.thing.target", "${actor} looks at you.");
            public static readonly string LookSelf = "system.look.thing";
            public static readonly MessageDefinition Goodbye = new MessageDefinition("system.goodbye.self", "Goodbye!");
        }

        public IViewManager ViewManager { get; set; }


        public IPlayerRepository PlayerRepository { get; set; }

        [Command]
        public void quit([Actor] Player player)
        {
            player.ToSelf(Messages.Goodbye);
            if (player.Client.State == ConnectedState.Playing)
            {
                PlayerRepository.Save(player);
            }
            // comment out for now
            //player.FirePlayerEvent(Player.PlayerEventType.Quiting);
            player.Client.Close();
        }

        [Command]
        public string look([Actor] Living actor)
        {
            string result = "";
            var room = actor.Room;
            if (room == null)
                return "";

            result += room.Name + "\r\n";
            result += room.ShortDescription + "\r\n";
            result += "\r\n";
            if (room.LivingThings.Count > 1)
            {
                result += "Players:\r\n";
                foreach (Living animate in room.LivingThings)
                {
                    if (animate != actor)
                    {
                        result += animate.Name + "\r\n";
                    }
                }
            }

            if (room.Items.Count > 0)
                result += ItemCommands.DisplayItemList("Items:", room.Items);

            if (room.Exits.Count > 0)
            {
                result += "Available Exits: [ ";
                foreach (RoomExit exit in room.Exits.Values)
                {
                    if (OpenableAttribute.IsOpen(exit))
                    {
                        result += exit.Direction;
                        result += " ";
                    }
                }
                result += "]\r\n";
            }
            else
            {
                result += "Available Exits: none\r\n.";
            }
            return result;
        }

        [Command]
        public void look([Actor] Living actor, string target)
        {
            if (actor.Room == null)
            {
                actor.ToSelf(CommonMessages.ErrorNotHere, target);
                
                return;
            }

            Living lookAt = actor.Room.LivingThings.FindOne(target);
            if (lookAt == null || ViewManager.GetVisibility(actor, lookAt) == VisiblityType.NotVisible)
            {
                // couldn't find them
                actor.ToSelf(CommonMessages.ErrorNotHere, target);
                return;
            }

            string text = "";
            text += ViewManager.GetTitle(actor, lookAt) + "\r\n";
            text += ViewManager.GetLong(actor, lookAt) + "\r\n";
            //TODO: create message namespace:
            actor.ToSelf(Messages.LookSelf, text, lookAt);
            actor.ToTarget(Messages.LookTarget, lookAt);
        }

        [CommandAttribute(Description="Lists commands available to a user")]
        public void commands([Actor] Player actor, string searchText)
        {
            var commandList = MethodInvoker.GetAvailableCommands(searchText);
            StringBuilder sb = new StringBuilder();

            SortedList<string, ICommand> list = FilterAndSortCommands(commandList, actor);
            
            foreach (string key in list.Keys)
            {
                ICommand cmd = list[key];
                sb.AppendLine(cmd.ShortHelp());
            }

            actor.ToSelf("misc.commands.search", sb.ToString());
        }

        [CommandAttribute(Description = "Lists commands available to a user")]
        public void commands([Actor] Player actor)
        {
            var commandList = MethodInvoker.GetAvailableCommands();
            StringBuilder sb = new StringBuilder();

            SortedList<string, ICommand> list = FilterAndSortCommands(commandList, actor);
            int i = 0;
            foreach (string key in list.Keys)
            {
                sb.AppendFormat("{0,-20} ", key);
                i++;
                if (i % 3 == 0)
                    sb.AppendLine();
            }
            if (i % 3 != 0)
                sb.AppendLine();

            actor.ToSelf("misc.commands.all", sb.ToString());
        }

        private SortedList<string, ICommand> FilterAndSortCommands(IEnumerable<ICommand> commands, Player actor)
        {
            SortedList<string, ICommand> list = new SortedList<string, ICommand>();
            foreach (ICommand cmd in commands)
            {
                if (cmd.CanInvoke(actor))
                {
                    string name = cmd.Aliases.Length > 0 ? cmd.Aliases[0] : cmd.Name;
                    list[name] = cmd;
                }
            }
            return list;
        }


    }
}

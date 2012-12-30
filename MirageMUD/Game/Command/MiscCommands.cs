using System.Collections.Generic;
using System.Text;
using Mirage.Game.Communication;
using Mirage.Game.World;
using Mirage.Game.World.Attribute;
using Mirage.Game.World.Query;
using Mirage.IO.Net;
using System.Collections;

namespace Mirage.Game.Command
{
    public class MiscCommands : CommandDefaults
    {       
        public IMessageFactory MessageFactory { get; set; }

        public IViewManager ViewManager { get; set; }

        public IPlayerRepository PlayerRepository { get; set; }
        /// <summary>
        ///     Say something to everyone in the room
        /// </summary>
        /// <param name="actor">the player speaking</param>
        /// <param name="args">the message to speak</param>
        /// <param name="extraArgs"></param>
        [Command(Aliases = new string[] { "'", "say" })]
        public IMessage say([Actor] Living actor, [CustomParse] string message)
        {
            //speak to all others in the room
            IMessage msgToOthersTemplate = MessageFactory.GetMessage("communication.SayOthers");
            msgToOthersTemplate["player"] = actor.Title;
            msgToOthersTemplate["message"] = message;
            foreach (Living am in actor.Room.LivingThings)
            {
                if (am != actor)
                {
                    if (am is Player && ((Player)am).CommunicationPreferences.IsIgnored(actor.Uri))
                        continue;
                    IMessage m = msgToOthersTemplate.Copy();
                    m["player"] = ViewManager.GetTitle(am, actor);
                    am.Write(m);
                }
            }

            //repeat message to yourself as confirmation
            IMessage msgToSelf = MessageFactory.GetMessage("communication.SaySelf");
            msgToSelf["message"] = message;
            return msgToSelf;
        }

        [Command]
        public IMessage tell([Actor] Living actor, string target, [CustomParse] string message)
        {
            // look up the target
            Player p = (Player)World.Players.FindOne(target, QueryMatchType.Exact);
            
            if (p == null)
            {
                // couldn't find them, send an error
                IMessage errorMsg = MessageFactory.GetMessage("common.error.PlayerNotPlaying");
                errorMsg["player"] = target;
                return errorMsg;
            }
            else
            {
                if (p.CommunicationPreferences.IsIgnored(actor.Uri) 
                    && !actor.Principal.IsInRole("immortal"))
                {
                    //They're ignoring us!
                    IMessage errorMsg = MessageFactory.GetMessage("communication.BeingIgnored");
                    errorMsg["player"] = target;
                    return errorMsg;
                } else {
                    // format the messages
                    IMessage msgToTarget = MessageFactory.GetMessage("communication.TellOthers");
                    msgToTarget["player"] = ViewManager.GetTitle(p, actor);
                    msgToTarget["message"] = message;
                    p.Write(msgToTarget);

                    IMessage msgToSelf = MessageFactory.GetMessage("communication.TellSelf");
                    msgToSelf["message"] = message;
                    msgToSelf["target"] = ViewManager.GetTitle(actor, p);

                    return msgToSelf;
                }
            }
        }

        [Command]
        public void quit([Actor] Player player)
        {
            player.Write(MessageFactory.GetMessage("system.goodbye"));
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

            result += room.Title + "\r\n";
            result += room.ShortDescription + "\r\n";
            result += "\r\n";
            if (room.LivingThings.Count > 1)
            {
                result += "Players:\r\n";
                foreach (Living animate in room.LivingThings)
                {
                    if (animate != actor)
                    {
                        result += animate.Title + "\r\n";
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
                IMessage message = MessageFactory.GetMessage("common.error.NotHere");
                message["target"] = target;
                actor.Write(message);
                return;
            }

            Living lookAt = actor.Room.LivingThings.FindOne(target);
            if (lookAt == null || ViewManager.GetVisibility(actor, lookAt) == VisiblityType.NotVisible)
            {
                // couldn't find them
                IMessage message = MessageFactory.GetMessage("common.error.NotHere");
                message["target"] = target;
                actor.Write(message);
                return;
            }

            string text = "";
            text += ViewManager.GetTitle(actor, lookAt) + "\r\n";
            text += ViewManager.GetLong(actor, lookAt) + "\r\n";
            //TODO: create message namespace:
            actor.Write(MessageFactory.GetMessage("common.LookPlayer", text));
            lookAt.Write(MessageFactory.GetMessage("common.PlayerLooksAtYou", ViewManager.GetTitle(lookAt, actor) + " looks at you.\r\n"));
        }

        [CommandAttribute(Description="Lists commands available to a user")]
        public IMessage commands([Actor] Player actor, string searchText)
        {
            var commandList = MethodInvoker.GetAvailableCommands(searchText);
            StringBuilder sb = new StringBuilder();

            SortedList<string, ICommand> list = FilterAndSortCommands(commandList, actor);
            
            foreach (string key in list.Keys)
            {
                ICommand cmd = list[key];
                sb.AppendLine(cmd.ShortHelp());
            }

            return MessageFactory.GetMessage("common.CommandListShort", sb.ToString());
        }

        [CommandAttribute(Description = "Lists commands available to a user")]
        public IMessage commands([Actor] Player actor)
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

            return MessageFactory.GetMessage("common.CommandListAll", sb.ToString());
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

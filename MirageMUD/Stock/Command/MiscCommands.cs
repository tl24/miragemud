using System;
using System.Collections.Generic;
using System.Text;
using Mirage.Core.Command;
using Mirage.Core.Communication;
using Mirage.Core.Data;
using Mirage.Core.Data.Query;
using Mirage.Core.IO;
using Mirage.Core.Data.Attribute;
using Mirage.Stock.Data;
using Mirage.Stock.Data.Items;
using Mirage.Core.Data.Containers;

namespace Mirage.Stock.Command
{
    public class MiscCommands
    {

        private IQueryManager _queryManager;
        private IMessageFactory _messageFactory;
        private IViewManager _viewManager;
        private IPlayerRepository _playerRepository;

        public IQueryManager QueryManager
        {
            get { return this._queryManager; }
            set { this._queryManager = value; }
        }

        public IMessageFactory MessageFactory
        {
            get { return _messageFactory; }
            set { _messageFactory = value; }
        }

        public IViewManager ViewManager
        {
            get { return _viewManager; }
            set { _viewManager = value; }
        }

        public IPlayerRepository PlayerRepository
        {
            get { return _playerRepository; }
            set { _playerRepository = value; }
        }
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
            ResourceMessage msgToOthersTemplate = (ResourceMessage)MessageFactory.GetMessage("msg:/communication/say.others");
            msgToOthersTemplate["player"] = actor.Title;
            msgToOthersTemplate["message"] = message;
            foreach (Living am in actor.Container.Contents(typeof(Living)))
            {
                if (am != actor)
                {
                    if (am is Player && ((Player)am).CommunicationPreferences.IsIgnored(actor.Uri))
                        continue;
                    ResourceMessage m = (ResourceMessage) msgToOthersTemplate.Copy();
                    m["player"] = ViewManager.GetTitle(am, actor);
                    am.Write(m);
                }
            }

            //repeat message to yourself as confirmation
            ResourceMessage msgToSelf = (ResourceMessage) MessageFactory.GetMessage("msg:/communication/say.self");
            msgToSelf["message"] = message;
            return msgToSelf;
        }

        [Command]
        public IMessage tell([Actor] Living actor, string target, [CustomParse] string message)
        {
            // look up the target
            Player p = (Player)QueryManager.Find(new ObjectQuery(null, "/Players", new ObjectQuery(target)));
            if (p == null)
            {
                // couldn't find them, send an error
                ResourceMessage errorMsg = (ResourceMessage) MessageFactory.GetMessage("msg:/common/error/PlayerNotPlaying");
                errorMsg["player"] = target;
                return errorMsg;
            }
            else
            {
                if (p.CommunicationPreferences.IsIgnored(actor.Uri) 
                    && !actor.Principal.IsInRole("immortal"))
                {
                    //They're ignoring us!
                    ResourceMessage errorMsg = (ResourceMessage)MessageFactory.GetMessage("msg:/communication/being.ignored");
                    errorMsg["player"] = target;
                    return errorMsg;
                } else {
                    // format the messages
                    ResourceMessage msgToTarget = (ResourceMessage)MessageFactory.GetMessage("msg:/communication/tell.others");
                    msgToTarget["player"] = ViewManager.GetTitle(p, actor);
                    msgToTarget["message"] = message;
                    p.Write(msgToTarget);

                    ResourceMessage msgToSelf = (ResourceMessage)MessageFactory.GetMessage("msg:/communication/tell.self");
                    msgToSelf["message"] = message;
                    msgToSelf["target"] = ViewManager.GetTitle(actor, p);

                    return msgToSelf;
                }
            }
        }

        [Command]
        public void quit([Actor] Player player)
        {
            player.Write(MessageFactory.GetMessage("msg:/system/goodbye"));
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
            IViewable viewableContainer = actor.Container as IViewable;
            if (viewableContainer != null)
            {
                result += viewableContainer.Title + "\r\n";
                result += viewableContainer.ShortDescription + "\r\n";
                result += "\r\n";
            }
            if (actor.Container is Room)
            {
                Room room = actor.Container as Room;
                if (room.Animates.Count > 1)
                {
                    result += "Players:\r\n";
                    foreach (Living animate in room.Animates)
                    {
                        if (animate != actor)
                        {
                            result += animate.Title + "\r\n";
                        }
                    }
                }

                if (room.Items.Count > 0)
                {
                    result += "Items:\r\n";
                    foreach (ItemBase item in room.Items)
                        result += item.ShortDescription + "\r\n";
                }

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
            }
            return result;
        }

        [Command]
        public void look([Actor] Living actor, string target)
        {
            Living lookAt = QueryManager.Find(actor.Container, ObjectQuery.parse("Animates", target)) as Living;
            if (lookAt == null || ViewManager.GetVisibility(actor, lookAt) == VisiblityType.NotVisible)
            {
                // couldn't find them
                ResourceMessage rm = (ResourceMessage) MessageFactory.GetMessage("msg:/common/error/NotHere");
                rm["target"] = target;
                actor.Write(rm);
                return;
            }

            string text = "";
            text += ViewManager.GetTitle(actor, lookAt) + "\r\n";
            text += ViewManager.GetLong(actor, lookAt) + "\r\n";
            //TODO: create message namespace:
            actor.Write(new StringMessage(MessageType.Information, Namespaces.Common, "look.player", text));
            lookAt.Write(new StringMessage(MessageType.Information, Namespaces.Common, "look.target", ViewManager.GetTitle(lookAt, actor) + " looks at you.\r\n"));
        }

        [CommandAttribute(Description="Lists commands available to a user")]
        public IMessage commands([Actor] Player actor, string searchText)
        {
            IList<ICommand> commandList = MethodInvoker.GetAvailableCommands(searchText);
            StringBuilder sb = new StringBuilder();

            SortedList<string, ICommand> list = FilterAndSortCommands(commandList, actor);
            
            foreach (string key in list.Keys)
            {
                ICommand cmd = list[key];
                sb.AppendLine(cmd.ShortHelp());
            }

            return new StringMessage(MessageType.Information, Namespaces.Common, "command.list.short", sb.ToString());
        }

        [CommandAttribute(Description = "Lists commands available to a user")]
        public IMessage commands([Actor] Player actor)
        {
            IList<ICommand> commandList = MethodInvoker.GetAvailableCommands();
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

            return new StringMessage(MessageType.Information, Namespaces.Common, "command.list.all", sb.ToString());
        }

        private SortedList<string, ICommand> FilterAndSortCommands(IList<ICommand> commands, Player actor)
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


        [CommandAttribute(Aliases=new string[]{"get"})]
        public IMessage get_item([Actor] Living actor, string target)
        {
            ItemBase item = QueryManager.Find(actor.Container, ObjectQuery.parse("Items", target)) as ItemBase;
            if (item == null)
                return new StringMessage(MessageType.PlayerError, "item.not.here", "I don't see that here.\r\n");

            if (actor.CanAdd(item))
            {
                ContainerUtils.Transfer(item, actor);
                foreach (Living am in actor.Container.Contents(typeof(Living)))
                {
                    if (am != actor)
                    {
                        am.Write(new StringMessage(MessageType.Information, "get.item.other", actor.Title + " gets " + item.ShortDescription + "\r\n"));
                    }
                }
                return new StringMessage(MessageType.Confirmation, "get.item.self", "You get " + item.ShortDescription + "\r\n");
            } else {
                return new StringMessage(MessageType.PlayerError, "get.item.error", "You can't pick that up!\r\n");
            }
        }

        [Command]
        public IMessage drop([Actor] Living actor, string target)
        {
            Room room = actor.Container as Room;            
            if (target.Equals("all", StringComparison.CurrentCultureIgnoreCase))
            {
                if (room == null)
                    return new StringMessage(MessageType.PlayerError, "cant.drop.all", "You can't drop anything here.\r\n");

                List<ItemBase> items = new List<ItemBase>(actor.Inventory);
                foreach (ItemBase item in items)
                    actor.Write(drop(actor, room, item));

                return null;
            }
            else
            {
                if (room == null)
                    return new StringMessage(MessageType.PlayerError, "cant.drop.that", "You can't drop that here.\r\n");

                ItemBase item = QueryManager.Find(actor, ObjectQuery.parse("Inventory", target)) as ItemBase;
                if (item == null)
                    return new StringMessage(MessageType.PlayerError, "dont.have.item", "You don't have that!\r\n");

                return drop(actor, room, item);
            }
        }

        public IMessage drop(Living actor, Room room, ItemBase item)
        {
            if (room.CanAdd(item))
            {
                ContainerUtils.Transfer(item, room);
                foreach (Living am in actor.Container.Contents(typeof(Living)))
                {
                    if (am != actor)
                    {
                        am.Write(new StringMessage(MessageType.Information, "drop.item.other", actor.Title + " drops " + item.ShortDescription + ".\r\n"));
                    }
                }
                return new StringMessage(MessageType.Confirmation, "drop.item.self", "You drop " + item.ShortDescription + ".\r\n");
            }
            else
            {
                return new StringMessage(MessageType.PlayerError, "drop.item.error", "You can't drop " + item.ShortDescription + "!\r\n");
            }
        }
    }
}

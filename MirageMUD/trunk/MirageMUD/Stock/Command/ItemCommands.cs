using System;
using System.Collections.Generic;
using System.Text;
using Mirage.Core.Command;
using Mirage.Core.Communication;
using Mirage.Core.Data.Query;
using Mirage.Core.Data.Containers;
using Mirage.Stock.Data;
using Mirage.Stock.Data.Items;

namespace Mirage.Stock.Command
{
    public class ItemCommands
    {
        private IQueryManager _queryManager;
        public IQueryManager QueryManager
        {
            get { return this._queryManager; }
            set { this._queryManager = value; }
        }

        [CommandAttribute(Aliases = new string[] { "get" })]
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
            }
            else
            {
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

        [CommandAttribute(Description="Wear an item")]
        public IMessage wear([Actor] Living actor, string target)
        {
            ItemBase item = QueryManager.Find(actor, ObjectQuery.parse("Inventory", target)) as ItemBase;
            if (item == null)
                return new StringMessage(MessageType.PlayerError, "item.not.found", "You don't have that.\r\n");

            Armor armor = item as Armor;
            if (armor == null)
                return new StringMessage(MessageType.PlayerError, "item.not.wearable", "You can't wear that.\r\n");

            Armor removed = actor.EquipItem(armor, true);
            if (removed != null)
                actor.Write(new StringMessage(MessageType.Information, "item.removed", "You remove " + removed.ShortDescription + ".\r\n"));

            return new StringMessage(MessageType.Information, "item.worn", "You wear " + armor.ShortDescription + ".\r\n");
        }

        [CommandAttribute(Description = "Remove an item")]
        public IMessage remove([Actor] Living actor, string target)
        {
            Armor item = QueryManager.Find(actor, ObjectQuery.parse("Equipment", target)) as Armor;
            if (item == null)
                return new StringMessage(MessageType.PlayerError, "item.not.worn", "You're not wearing that!\r\n");

            actor.UnequipItem(item);
            return new StringMessage(MessageType.Information, "item.removed", "You remove " + item.ShortDescription + ".\r\n");

        }

        [CommandAttribute(Description="Shows the equipment currently being worn")]
        public IMessage equipment([Actor] Living actor)
        {
            WearLocations[] locs = (WearLocations[]) Enum.GetValues(typeof(WearLocations));
            StringBuilder sb = new StringBuilder();
            WornItems eq = actor.Equipment;

            foreach (WearLocations loc in locs)
            {
                Armor item = eq.GetItemAt(loc);
                string locString = string.Format("<{0}>", loc);
                string desc = item != null ? item.ShortDescription : "none";
                sb.Append(string.Format("{0,-15} {1}\r\n", locString, desc));    
            }

            return new StringMessage(MessageType.Information, "equipment", sb.ToString());
        }
    }
}

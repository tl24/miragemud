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

        private IMessageFactory _messageFactory;
        public IMessageFactory MessageFactory
        {
            get { return this._messageFactory; }
            set { this._messageFactory = value; }
        }

        [CommandAttribute(Aliases = new string[] { "get" })]
        public IMessage get_item([Actor] Living actor, string target)
        {
            if ("all".Equals(target, StringComparison.CurrentCultureIgnoreCase)) {
                foreach(ItemBase item in new List<ItemBase>(actor.Container.Contents<ItemBase>())) {
                    actor.Write(get_item(actor, item));
                }
                return null;
            } else {
                ItemBase item = QueryManager.Find(actor.Container, ObjectQuery.parse("Items", target)) as ItemBase;
                if (item == null)
                    return MessageFactory.GetMessage("item.error.ItemNotHere", "I don't see that here.\r\n");
                else
                    return get_item(actor, item);
            }
        }

        public IMessage get_item(Living actor, ItemBase item) {
            if (actor.CanAdd(item))
            {
                ContainerUtils.Transfer(item, actor);
                foreach (Living am in actor.Container.Contents(typeof(Living)))
                {
                    if (am != actor)
                    {
                        am.Write(MessageFactory.GetMessage("item.PlayerGetsItem", actor.Title + " gets " + item.ShortDescription + "\r\n"));
                    }
                }
                return MessageFactory.GetMessage("item.YouGetItem", "You get " + item.ShortDescription + "\r\n");
            }
            else
            {
                return MessageFactory.GetMessage("item.error.CantGetItem", "You can't pick up " + item.ShortDescription + "!\r\n");
            }
        }

        [Command]
        public IMessage drop([Actor] Living actor, string target)
        {
            Room room = actor.Container as Room;
            if (target.Equals("all", StringComparison.CurrentCultureIgnoreCase))
            {
                if (room == null)
                    return MessageFactory.GetMessage("item.error.CantDropAll", "You can't drop anything here.\r\n");

                List<ItemBase> items = new List<ItemBase>(actor.Inventory);
                foreach (ItemBase item in items)
                    actor.Write(drop(actor, room, item));

                return null;
            }
            else
            {
                if (room == null)
                    return MessageFactory.GetMessage("item.error.CantDropItem", "You can't drop that here.\r\n");

                ItemBase item = QueryManager.Find(actor, ObjectQuery.parse("Inventory", target)) as ItemBase;
                if (item == null)
                    return MessageFactory.GetMessage("item.error.DontHaveItem", "You don't have that!\r\n");

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
                        am.Write(MessageFactory.GetMessage("item.PlayerDropsItem", actor.Title + " drops " + item.ShortDescription + ".\r\n"));
                    }
                }
                return MessageFactory.GetMessage("item.YouDropItem", "You drop " + item.ShortDescription + ".\r\n");
            }
            else
            {
                return MessageFactory.GetMessage("item.error.CantDropItem", "You can't drop that here.\r\n");
            }
        }

        [CommandAttribute(Description="Wear an item")]
        public IMessage wear([Actor] Living actor, string target)
        {
            ItemBase item = QueryManager.Find(actor, ObjectQuery.parse("Inventory", target)) as ItemBase;
            if (item == null)
                return MessageFactory.GetMessage("item.error.DontHaveItem", "You don't have that!\r\n");

            Armor armor = item as Armor;
            if (armor == null)
                return MessageFactory.GetMessage("item.error.CantWearItem", "You can't wear that!\r\n");

            Armor removed = actor.EquipItem(armor, true);
            if (removed != null)
            {
                actor.Write(MessageFactory.GetMessage("item.YouRemoveItem", "You remove " + removed.ShortDescription + ".\r\n"));
                if (actor.Container is IReceiveMessages)
                    ((IReceiveMessages)actor.Container).Write(actor, MessageFactory.GetMessage("item.PlayerRemovesItem", actor.Title + " removes " + removed.ShortDescription + ".\r\n"));
            }
            if (actor.Container is IReceiveMessages)
                ((IReceiveMessages)actor.Container).Write(actor, MessageFactory.GetMessage("item.PlayerWearsItem", actor.Title + " wears " + removed.ShortDescription + ".\r\n"));
            return MessageFactory.GetMessage("item.YouWearItem", "You wear " + armor.ShortDescription + ".\r\n");
        }

        [CommandAttribute(Description = "Remove an item")]
        public IMessage remove([Actor] Living actor, string target)
        {
            Armor item = QueryManager.Find(actor, ObjectQuery.parse("Equipment", target)) as Armor;
            if (item == null)
                return MessageFactory.GetMessage("item.error.ItemNotWorn", "You're not wearing that!\r\n");

            actor.UnequipItem(item);
            if (actor.Container is IReceiveMessages)
                ((IReceiveMessages)actor.Container).Write(actor, MessageFactory.GetMessage("item.PlayerRemovesItem", actor.Title + " removes " + item.ShortDescription + ".\r\n"));
            return MessageFactory.GetMessage("item.YouRemoveItem", "You remove " + item.ShortDescription + ".\r\n");

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

            return MessageFactory.GetMessage("item.Equipment", sb.ToString());
        }

        [CommandAttribute(Description="Shows the current items in inventory that a player has")]
        public IMessage inventory([Actor] Living actor)
        {
            return MessageFactory.GetMessage("item.Inventory", DisplayItemList("You have the following items:", actor.Inventory));
        }

        public static string DisplayItemList(string title, ICollection<ItemBase> items)
        {
            string result = "";
            if (!string.IsNullOrEmpty(title))
                result += title + "\r\n";

            foreach (ItemBase item in items)
                result += item.ShortDescription + "\r\n";

            return result;
        }

    }
}

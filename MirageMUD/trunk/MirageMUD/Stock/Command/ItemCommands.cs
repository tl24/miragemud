using System;
using System.Collections.Generic;
using System.Text;
using Mirage.Core.Command;
using Mirage.Core.Communication;
using Mirage.Core.Data.Query;
using Mirage.Core.Data.Containers;
using Mirage.Core.Data;
using Mirage.Core.Data.Items;

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
        public void get_item([Actor] Living actor, string target)
        {
            if ("all".Equals(target, StringComparison.CurrentCultureIgnoreCase)) {
                foreach(ItemBase item in new List<ItemBase>(actor.Container.Contents<ItemBase>())) {
                    get_item(actor, item);
                }
            } else {
                ItemBase item = QueryManager.Find(actor.Container, ObjectQuery.parse("Items", target)) as ItemBase;
                if (item == null)
                    actor.Write("item.error.nothere.self", "I don't see that here.\r\n");
                else
                    get_item(actor, item);
            }
        }

        public void get_item(Living actor, ItemBase item) {
            if (actor.CanAdd(item))
            {
                ContainerUtils.Transfer(item, actor);
                actor.ToRoom("item.get", "${actor} gets ${object.short}", null, item);
                actor.ToSelf("item.get", "You get ${object.short}", null, item);
            }
            else
            {
                actor.ToSelf("item.error.cantgetitem.self", "You can't pick up ${object.short}!", null, item); 
            }
        }

        [Command]
        public void drop([Actor] Living actor, string target)
        {
            Room room = actor.Container as Room;
            if (target.Equals("all", StringComparison.CurrentCultureIgnoreCase))
            {
                if (room == null)
                {
                    actor.Write("item.error.cantdropall.self", "You can't drop anything here.\r\n");
                    return;
                }

                List<ItemBase> items = new List<ItemBase>(actor.Inventory);
                foreach (ItemBase item in items)
                    drop(actor, room, item);

            }
            else
            {
                if (room == null)
                {
                    actor.Write("item.error.cantdrop.self", "You can't drop that here.\r\n");
                    return;
                }

                ItemBase item = QueryManager.Find(actor, ObjectQuery.parse("Inventory", target)) as ItemBase;
                if (item == null)
                {
                    actor.Write("item.error.donthaveitem.self", "You don't have that.\r\n");
                    return;
                }
                drop(actor, room, item);
            }
        }

        /// <summary>
        /// Drops a single item into the room
        /// </summary>
        /// <param name="actor">the actor dropping the item</param>
        /// <param name="room">the room to drop it in</param>
        /// <param name="item">the item being dropped</param>
        /// <returns>true if successfully dropped, false otherwise</returns>
        public bool drop(Living actor, Room room, ItemBase item)
        {
            if (room.CanAdd(item))
            {
                ContainerUtils.Transfer(item, room);
                actor.ToRoom("item.drop", "${actor} drops ${object.short}.", null, item);
                actor.ToSelf("item.drop", "You drop ${object.short}.", null, item);
                return true;
            }
            else
            {
                actor.Write("item.error.cantdrop.self", "You can't drop that here.\r\n");
                return false;
            }
        }

        [CommandAttribute(Description="Wear an item")]
        public void wear([Actor] Living actor, string target)
        {
            ItemBase item = QueryManager.Find(actor, ObjectQuery.parse("Inventory", target)) as ItemBase;
            if (item == null)
            {
                actor.Write("item.error.donthaveitem.self", "You don't have that!\r\n");
                return;
            }
            Armor armor = item as Armor;
            if (armor == null)
            {
                actor.Write("item.error.cantwearitem.self", "You can't wear that!\r\n");
                return;
            }

            Armor removed = actor.EquipItem(armor, true);
            if (removed != null)
            {
                actor.ToSelf("item.remove", "You remove ${object.short}.", null, removed);
                actor.ToRoom("item.remove", "${actor} removes ${object.short}.", null, removed);
            }
            actor.ToSelf("item.wear", "You wear ${object.short}.", null, armor);
            actor.ToRoom("item.remove", "${actor} wears ${object.short}.", null, armor);
        }

        [CommandAttribute(Description = "Remove an item")]
        public void remove([Actor] Living actor, string target)
        {
            Armor item = QueryManager.Find(actor, ObjectQuery.parse("Equipment", target)) as Armor;
            if (item == null)
            {
                actor.Write("item.error.itemnotworn.self", "You're not wearing that!\r\n");
                return;
            }

            actor.UnequipItem(item);
            actor.ToSelf("item.remove", "You remove ${object.short}.", null, item);
            actor.ToRoom("item.remove", "${actor} removes ${object.short}.", null, item);
        }

        [CommandAttribute(Description="Shows the equipment currently being worn")]
        public void equipment([Actor] Living actor)
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
            actor.Write("item.equipment", sb.ToString());
        }

        [CommandAttribute(Description="Shows the current items in inventory that a player has")]
        public void inventory([Actor] Living actor)
        {         
            actor.Write("item.inventory", DisplayItemList("You have the following items:", actor.Inventory));
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

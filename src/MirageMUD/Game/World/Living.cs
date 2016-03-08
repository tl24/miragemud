using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Principal;
using System.Xml.Serialization;
using JsonExSerializer;
using Mirage.Game.Communication;
using Mirage.Game.World.Containers;
using Mirage.Game.World.Items;
using Mirage.Game.World.Query;
using Mirage.Core.Messaging;
using Mirage.Core.Command;

namespace Mirage.Game.World
{
    /// <summary>
    ///     The gender for a Living (Player or Mobile)
    /// </summary>
    public enum GenderType {
        /// <summary>
        ///     Male
        /// </summary>
        Male,

        /// <summary>
        ///     Female
        /// </summary>
        Female,

        /// <summary>
        ///     Other or Unknown
        /// </summary>
        Other
    }

    /// <summary>
    ///     A base class for living breathing things such as players
    /// and mobiles.
    /// </summary>
    public abstract class Living : LivingTemplateBase, IActor, IReceiveMessages, IContainer, IContainable
    {
        protected IContainer _itemContainer;

        public Living()
        {
            Inventory = new LinkedList<ItemBase>();
            _itemContainer = new GenericCollectionContainer<ItemBase>(Inventory, this);
            Equipment = new WornItems();
        }

        public abstract void Write(IMessage message);

        public abstract void Write(object sender, IMessage message);

        public abstract IPrincipal Principal { get; }

        [XmlIgnore]
        [JsonExIgnore]
        public Room Room
        {
            get
            {
                return Container as Room;
            }
            set
            {
                Container = value;
            }
        }

        [XmlIgnore]
        [JsonExIgnore]
        public IContainer Container { get; set; }

        public ICollection<ItemBase> Inventory { get; private set; }

        #region Equipment
        /// <summary>
        /// Equipment that is currently being worn on the body
        /// </summary>
        public WornItems Equipment { get; private set; }

        /// <summary>
        /// Equips an item from the inventory.  If an item is already worn in the same location that the
        /// new item will be worn in, it will be replaced if <paramref name="replace"/> is true, otherwise
        /// the operation will fail
        /// </summary>
        /// <param name="item">the item to wear</param>
        /// <param name="replace">true to replace an existing item in the same spot</param>
        /// <returns>the item replaced if any</returns>
        public Armor EquipItem(Armor item, bool replace)
        {
            // Items worn must be in inventory
            if (!Inventory.Remove(item))
                throw new InvalidOperationException("Item cannot be worn because it is not in the inventory, " + item.Uri);

            Armor existing = null;
            if (replace)
                existing = Equipment.Remove(item.WearFlags);

            Equipment.Add(item);

            // add old item back to inventory
            if (existing != null)
                Inventory.Add(existing);

            return existing;
        }

        /// <summary>
        /// Equips all wearable items in the inventory for which their are open wear slots
        /// </summary>
        /// <returns>Returns a list of the items that were worn</returns>
        public ICollection<Armor> EquipAll()
        {
            List<Armor> result = new List<Armor>();
            foreach (ItemBase item in Inventory)
            {
                //only consider armor
                Armor armor = item as Armor;
                if (armor == null)
                    continue;

                if (Equipment.IsOpen(armor.WearFlags))
                {
                    Equipment.Add(armor);
                    result.Add(armor);
                }
            }
            foreach (Armor worn in result)
                Inventory.Remove(worn);

            return result;
        }

        /// <summary>
        /// Removes a worn item
        /// </summary>
        /// <param name="item">the worn item</param>
        public void UnequipItem(Armor item)
        {
            if (!Equipment.Remove(item))
                throw new InvalidOperationException("Item is not worn by this player or mobile");

            Inventory.Add(item);
        }

        /// <summary>
        /// Removes all worn items
        /// </summary>
        public void UnequipAll()
        {
            foreach (Armor item in Equipment)
            {
                Inventory.Add(item);
            }
            Equipment.Clear();
        }
        #endregion

        #region IContainer Members

        int IContainer.Count
        {
            get
            {
                return _itemContainer.Count;
            }
        }

        public void Add(object item)
        {
            _itemContainer.Add(item);
        }

        public void Remove(object item)
        {
            _itemContainer.Remove(item);
        }

        public bool Contains(object item)
        {
            return _itemContainer.Contains(item);
        }

        public bool CanAdd(object item)
        {
            return _itemContainer.CanAdd(item);
        }

        public IEnumerator GetEnumerator()
        {
            return _itemContainer.GetEnumerator();
        }

        #endregion
    }
}

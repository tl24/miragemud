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
    public abstract class Living : LivingTemplateBase, IContainable, IActor, IReceiveMessages, IContainer
    {
        private Room _room;
        private LinkedList<ItemBase> _inventory;
        protected IContainer _itemContainer;
        protected WornItems _equipment;

        public Living()
        {
            _inventory = new LinkedList<ItemBase>();
            _uriChildCollections.Add("Inventory", new ChildCollectionPair(_inventory, QueryHints.DefaultPartialMatch));
            _itemContainer = new GenericCollectionContainer<ItemBase>(_inventory, this);
            _equipment = new WornItems();
            _uriChildCollections.Add("Equipment", new ChildCollectionPair(_equipment, QueryHints.DefaultPartialMatch));
        }

        public abstract void Write(IMessage message);

        public abstract void Write(object sender, IMessage message);

        public abstract IPrincipal Principal { get; }


        #region IContainable Members

        [XmlIgnore]
        [JsonExIgnore]
        public IContainer Container
        {
            get { return _room; }
            set { _room = (Room)value; }
        }

        public bool CanBeContainedBy(IContainer container)
        {
            return (container is Room);
        }

        #endregion

        public ICollection<ItemBase> Inventory
        {
            get { return _inventory; }
        }

        #region Equipment
        /// <summary>
        /// Equipment that is currently being worn on the body
        /// </summary>
        public WornItems Equipment
        {
            get { return _equipment; }
        }

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

        public void Add(IContainable item)
        {
            _itemContainer.Add(item);
        }

        public void Remove(IContainable item)
        {
            _itemContainer.Remove(item);
        }

        public bool Contains(IContainable item)
        {
            return _itemContainer.Contains(item);
        }

        public bool CanContain(Type item)
        {
            return _itemContainer.CanContain(item);
        }

        public bool CanAdd(IContainable item)
        {
            return _itemContainer.CanAdd(item);
        }

        public IEnumerable Contents(Type t)
        {
            return _itemContainer.Contents(t);
        }

        public IEnumerable Contents()
        {
            return _itemContainer.Contents();
        }

        public System.Collections.Generic.IEnumerable<T> Contents<T>()
        {
            return _itemContainer.Contents<T>();
        }
        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using Mirage.Core.Communication;
using System.Xml.Serialization;
using Mirage.Core.Data.Query;
using JsonExSerializer;
using Mirage.Core.Data;
using System.Security.Principal;
using Mirage.Stock.Data.Items;
using System.Collections;
using Mirage.Core.Data.Containers;

namespace Mirage.Stock.Data
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

        public Living()
        {
            _inventory = new LinkedList<ItemBase>();
            _uriChildCollections.Add("Inventory", new ChildCollectionPair(_inventory, QueryHints.DefaultPartialMatch));
            _itemContainer = new GenericCollectionContainer<ItemBase>(_inventory, this);
        }

        public abstract void Write(IMessage message);

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
        #endregion
    }
}

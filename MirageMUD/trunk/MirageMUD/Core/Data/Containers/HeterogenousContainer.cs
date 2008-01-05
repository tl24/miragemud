using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.Specialized;

namespace Mirage.Core.Data.Containers
{
    /// <summary>
    /// A container class helper that stores multiple types of items, each in
    /// their own container
    /// </summary>
    public class HeterogenousContainer : IContainer
    {
        private Dictionary<Type, IContainer> _collections;
        private IContainer _parentContainer;

        public HeterogenousContainer()
        {
            _collections = new Dictionary<Type, IContainer>();
            _parentContainer = this;
        }

        public HeterogenousContainer(IContainer parentContainer)
        {
            _collections = new Dictionary<Type, IContainer>();
            _parentContainer = parentContainer;
        }

        public void AddContainer(Type baseType, IContainer container)
        {
            _collections[baseType] = container;
        }


        private IContainer FindContainer(IContainable item)
        {
            return FindContainer(item.GetType());
        }

        private IContainer FindContainer(Type itemType)
        {
            IContainer result = null;

            while (itemType != null && itemType != typeof(object)) {
                if (_collections.TryGetValue(itemType, out result))
                    break;
                itemType = itemType.BaseType;
            }
            return result;
        }

        #region IContainer Members

        public void Add(IContainable item)
        {
            IContainer itemContainer = FindContainer(item);
            if (itemContainer == null)
                throw new ContainerAddException("item could not be added", this, item);

            if (item.Container != this.ParentContainer || !itemContainer.Contains(item))
            {
                itemContainer.Add(item);
                item.Container = this.ParentContainer;
            }
        }

        public void Remove(IContainable item)
        {
            IContainer itemContainer = FindContainer(item);
            if (itemContainer == null)
                return;

            itemContainer.Remove(item);
            if (item.Container == this.ParentContainer)
                item.Container = null;
        }

        public bool Contains(IContainable item)
        {
            IContainer itemContainer = FindContainer(item);
            if (itemContainer == null)
                return false;
            return itemContainer.Contains(item);
        }

        public bool CanContain(Type item)
        {
            IContainer itemContainer = FindContainer(item);
            return itemContainer != null;
        }

        public bool CanAdd(IContainable item)
        {
            IContainer itemContainer = FindContainer(item);
            if (itemContainer != null)
                return itemContainer.CanAdd(item);
            else
                return false;
        }

        public System.Collections.IEnumerable Contents(Type t)
        {
            foreach (IContainer container in _collections.Values)
            {
                foreach (object item in container.Contents(t))
                    yield return item;
            }
        }

        public System.Collections.IEnumerable Contents()
        {
            return Contents(typeof(object));
        }

        public IContainer ParentContainer
        {
            get { return this._parentContainer; }
            set { this._parentContainer = value; }
        }
        #endregion
    }
}

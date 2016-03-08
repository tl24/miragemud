using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections;

namespace Mirage.Game.World.Containers
{
    /// <summary>
    /// A container class helper that stores multiple types of items, each in
    /// their own container
    /// </summary>
    public class HeterogenousContainer : IContainer
    {
        private Dictionary<Type, IContainer> _collections;

        public HeterogenousContainer()
        {
            _collections = new Dictionary<Type, IContainer>();
        }

        public void AddContainer<T>(IContainer<T> container)
        {
            AddContainer(typeof(T), container);            
        }

        public void AddContainer<T>(IContainer container)
        {
            AddContainer(typeof(T), container);
        }

        public void AddContainer(Type baseType, IContainer container)
        {
            _collections.Add(baseType, container);
        }

        private IContainer FindContainer(object item)
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

        public void Add(object item)
        {
            IContainer itemContainer = FindContainer(item);
            if (itemContainer == null)
                throw new ContainerAddException("item could not be added", this, item);

            itemContainer.Add(item);
        }

        public void Remove(object item)
        {
            IContainer itemContainer = FindContainer(item);
            if (itemContainer == null)
                return;

            itemContainer.Remove(item);
        }

        public bool Contains(object item)
        {
            IContainer itemContainer = FindContainer(item);
            if (itemContainer == null)
                return false;
            return itemContainer.Contains(item);
        }

        public bool CanAdd(object item)
        {
            IContainer itemContainer = FindContainer(item);
            if (itemContainer != null)
                return itemContainer.CanAdd(item);
            else
                return false;
        }

        public IEnumerator GetEnumerator()
        {
            var q =  from container in _collections.Values
                   from object item in container
                   select item;
            foreach (var i in q)
                yield return i;
        }

        public int Count
        {
            get
            {
                return _collections.Values.Sum(c => c.Count);
            }
        }
    }
}

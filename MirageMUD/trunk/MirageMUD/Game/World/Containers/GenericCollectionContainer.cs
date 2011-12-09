using System;
using System.Collections;
using System.Collections.Generic;

namespace Mirage.Game.World.Containers
{
    public class GenericCollectionContainer<T> : IContainer
    {
        private ICollection<T> _items;
        private IContainer _parentContainer;

        public GenericCollectionContainer(ICollection<T> items)
        {
            _items = items;
            _parentContainer = this;
        }

        public GenericCollectionContainer(ICollection<T> items, IContainer parentContainer)
        {
            _items = items;
            _parentContainer = parentContainer;
        }


        #region IContainer Members

        public virtual void Add(IContainable item)
        {
            if (ParentContainer.CanAdd(item))
            {
                    if (item.Container != this.ParentContainer || !this.Items.Contains((T)item))
                    {
                        _items.Add((T)item);
                        item.Container = this.ParentContainer;
                    }
            }
            else
            {
                throw new ContainerAddException("item could not be added", this, item);
            }
        }

        public virtual void Remove(IContainable item)
        {
            if (ParentContainer.CanAdd(item))
            {
                this.Items.Remove((T)item);
                if (item.Container == this.ParentContainer)
                    item.Container = null;
            }
        }

        public virtual  bool Contains(IContainable item)
        {
            if (item is T)
                return Items.Contains((T)item);
            else
                return false;
        }

        public virtual bool CanContain(Type item)
        {
            return typeof(T).IsAssignableFrom(item);
        }

        public virtual bool CanAdd(IContainable item)
        {
            return (item is T);
        }

        public virtual IEnumerable Contents(Type t)
        {
            foreach (T item in Items)
            {
                if (t.IsInstanceOfType(item))
                    yield return item;
            }
        }

        public virtual System.Collections.Generic.IEnumerable<IT> Contents<IT>() 
        {
            foreach (T item in Items)
            {
                if (item is IT)
                {
                    object o = item;

                    yield return (IT) o;
                }
            }
        }

        public virtual IEnumerable Contents()
        {
            return Contents(typeof(object));
        }

        #endregion

        public ICollection<T> Items
        {
            get { return this._items; }
            set { this._items = value; }
        }

        public IContainer ParentContainer
        {
            get { return this._parentContainer; }
            set { this._parentContainer = value; }
        }
    }
}

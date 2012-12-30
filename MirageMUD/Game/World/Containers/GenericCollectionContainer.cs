using System;
using System.Collections;
using System.Collections.Generic;

namespace Mirage.Game.World.Containers
{
    public class CollectionModifyingEventArgs : EventArgs
    {
        public CollectionModifyingEventArgs(object item)
        {
            this.Item = item;
            this.Cancel = false;
        }

        /// <summary>
        /// Set to true to cancel the operation
        /// </summary>
        public bool Cancel { get; set; }

        /// <summary>
        /// The item changing in the collection
        /// </summary>
        public object Item { get; private set; }
    }

    public class CollectionModifiedEventArgs : EventArgs
    {
        public CollectionModifiedEventArgs(object item)
        {
            this.Item = item;
        }

        /// <summary>
        /// The item changing in the collection
        /// </summary>
        public object Item { get; private set; }
    }

    public class GenericCollectionContainer<T> : IContainer<T>
    {
        private ICollection<T> Items;
        public IContainer ParentContainer { get; set; }

        public event EventHandler<CollectionModifyingEventArgs> ItemAdding;
        public event EventHandler<CollectionModifiedEventArgs> ItemAdded;
        public event EventHandler<CollectionModifyingEventArgs> ItemRemoving;
        public event EventHandler<CollectionModifiedEventArgs> ItemRemoved;
        public GenericCollectionContainer(ICollection<T> items)
        {
            Items = items;
        }

        public GenericCollectionContainer(ICollection<T> items, IContainer parentContainer) : this(items)
        {
            ParentContainer = parentContainer;
        }

        public int Count
        {
            get
            {
                return Items.Count;
            }
        }

        public virtual void Add(T item)
        {
            if (CanAdd(item))
            {
                    if (!Items.Contains((T)item))
                    {
                        Items.Add((T)item);
                        if (item is IContainable)
                        {
                            ((IContainable)item).Container = ParentContainer;
                        }
                    }
            }
            else
            {
                throw new ContainerAddException("item could not be added", this, item);
            }
        }

        public virtual void Remove(T item)
        {
            if (CanRemove(item))
            {
                this.Items.Remove(item);
                if (item is IContainable && ((IContainable)item).Container == ParentContainer)
                {
                    ((IContainable)item).Container = null;
                }
            }
        }

        public virtual bool Contains(object item)
        {
            if (item is T)
                return Items.Contains((T)item);
            else
                return false;
        }

        public virtual bool Contains(T item)
        {
            return Items.Contains(item);
        }

        private bool CanAdd(T item)
        {
            return OnCollectionModifying(ItemAdding, item);
        }

        private bool CanRemove(T item)
        {
            return OnCollectionModifying(ItemAdding, item);
        }

        private bool OnCollectionModifying(EventHandler<CollectionModifyingEventArgs> evt, T item)
        {
            if (evt != null)
            {
                var e = new CollectionModifyingEventArgs(item);
                evt(this, e);
                return !e.Cancel;
            }
            return true;
        }

        public void Add(object item)
        {
            if (item is T)
                Add((T)item);
            else
                throw new ContainerAddException("item could not be added", this, item);
        }

        public void Remove(object item)
        {
            if (item is T)
                Remove((T)item);
        }

        public bool CanAdd(object item)
        {
            if (item is T)
                return CanAdd((T)item);
            else
                return false;
        }

        public IEnumerator GetEnumerator()
        {
            return Items.GetEnumerator();
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return Items.GetEnumerator();
        }
    }
}

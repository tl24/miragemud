using System;
using System.Collections.Generic;
using System.Text;

namespace Mirage.Core.Collections
{
    /// <summary>
    /// A list collection that sends events whenever the collection is modified.
    /// Subscribers should subscribe to the CollectionModified event to know when the collection gets modified.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ListWithEvents<T> : IList<T>, ICollectionEvents
    {
        private List<T> _list;
        public event EventHandler<CollectionEventArgs> CollectionModified;

        /// <summary>
        /// Construct the list with no elements and default capacity
        /// </summary>
        public ListWithEvents()
        {
            _list = new List<T>();
        }

        /// <summary>
        /// Construct the list initializing it with the items in the collection
        /// </summary>
        /// <param name="collection">collection to populate the list with</param>
        public ListWithEvents(IEnumerable<T> collection)
        {
            _list = new List<T>(collection);
        }

        /// <summary>
        /// Construct the list with the specified initial capacity
        /// </summary>
        /// <param name="capacity"></param>
        public ListWithEvents(int capacity)
        {
            _list = new List<T>(capacity);            
        }

        /// <summary>
        /// Searches the list using the specified match predicate to find the 
        /// item.
        /// </summary>
        /// <param name="match">predicate for determining a match</param>
        /// <returns>an item matching the criteria if found</returns>
        public T Find(Predicate<T> match)
        {
            return _list.Find(match);
        }

        /// <summary>
        /// Searches the list using the specified match predicate and
        /// returns all items that match in a list.
        /// </summary>
        /// <param name="match">predicate for determining a match</param>
        /// <returns>list of items matching the criteria if found</returns>
        public List<T> FindAll(Predicate<T> match)
        {
            return _list.FindAll(match);
            
        }

        #region IList<T> Members

        /// <summary>
        /// Finds the index of the item in the list, or returns -1 if not found
        /// </summary>
        /// <param name="item">the item to find</param>
        /// <returns>its index or -1 if the item is not found</returns>
        public int IndexOf(T item)
        {
            return _list.IndexOf(item);
        }

        /// <summary>
        /// Inserts an item at the specified index in the list
        /// </summary>
        /// <param name="index">the insertion point</param>
        /// <param name="item">the item to insert</param>
        public void Insert(int index, T item)
        {
            _list.Insert(index, item);
            OnCollectionChanged(CollectionEventType.Add, index);
        }

        /// <summary>
        /// Removes the item at the specified index
        /// </summary>
        /// <param name="index">the index to remove</param>
        public void RemoveAt(int index)
        {
            _list.RemoveAt(index);
            OnCollectionChanged(CollectionEventType.Remove, index);
        }

        /// <summary>
        /// gets or sets the item at the specified index in the list
        /// </summary>
        /// <param name="index">the 0-based index of the item</param>
        /// <returns>the item at the specified index</returns>
        public T this[int index]
        {
            get
            {
                return _list[index];
            }
            set
            {
                _list[index] = value;
                OnCollectionChanged(CollectionEventType.Update, index);
            }
        }

        #endregion

        #region ICollection<T> Members

        /// <summary>
        /// Adds the item at the end of the list
        /// </summary>
        /// <param name="item">item to add</param>
        public void Add(T item)
        {
            Insert(Count, item);
        }

        /// <summary>
        /// Removes all items from the list
        /// </summary>
        public void Clear()
        {
            _list.Clear();
            OnCollectionChanged(CollectionEventType.Cleared);
        }

        /// <summary>
        /// Returns true if the list contains the specified item
        /// </summary>
        /// <param name="item">item to search for</param>
        /// <returns>true if it is in the list</returns>
        public bool Contains(T item)
        {
            return _list.Contains(item);
        }

        /// <summary>
        /// Copies the list to an array of the same type of the members in the list
        /// </summary>
        /// <param name="array">the array to copy to</param>
        /// <param name="arrayIndex">the starting index of the copy</param>
        public void CopyTo(T[] array, int arrayIndex)
        {
            _list.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Gets the number of elements actually contained in the list
        /// </summary>
        public int Count
        {
            get { return _list.Count; }
        }

        /// <summary>
        /// Returns true if this list is readonly
        /// </summary>
        public bool IsReadOnly
        {
            get { return ((ICollection<T>)_list).IsReadOnly; }
        }

        /// <summary>
        /// Removes the specified item from the list
        /// </summary>
        /// <param name="item">the item to remove</param>
        /// <returns>true if the item was removed, false if the collection does not contain the item</returns>
        public bool Remove(T item)
        {
            int i = IndexOf(item);
            if (i != -1) 
                RemoveAt(i);
            return (i != -1);
        }

        #endregion

        #region IEnumerable<T> Members

        /// <summary>
        /// Returns an enumerator over the items in the list
        /// </summary>
        /// <returns>an enumerator</returns>
        public IEnumerator<T> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        /// <summary>
        /// Returns an enumerator over the items in the list
        /// </summary>
        /// <returns>an enumerator</returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        /// <summary>
        /// Helper method to fire an event
        /// </summary>
        /// <param name="type"></param>
        /// <param name="indices"></param>
        protected void OnCollectionChanged(CollectionEventType type, params int[] indices)
        {
            object key = null;
            switch (type)
            {
                case CollectionEventType.Add:
                case CollectionEventType.Remove:
                case CollectionEventType.Update:
                    key = indices[0];
                    break;
                case CollectionEventType.AddRange:
                case CollectionEventType.RemoveRange:
                case CollectionEventType.UpdateRange:
                    key = indices;
                    break;
                case CollectionEventType.Cleared:
                    key = null;
                    break;
            }
            if (CollectionModified != null)
                CollectionModified(this, new CollectionEventArgs(type, key));
        }
    }
}

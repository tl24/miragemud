using System;
using System.Collections.Generic;
using System.Text;

namespace Mirage.Core.Util
{
    /// <summary>
    /// Provides an implementation of a set
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Set<T> : ISet<T>
    {
        private Dictionary<T, bool> _dictionary;

        /// <summary>
        /// Creates an empty set with the default equality comparer
        /// for the item type
        /// </summary>
        public Set()
        {
            _dictionary = new Dictionary<T, bool>();
        }

        /// <summary>
        /// Creates an empty set that uses the specified equality comparer to compare
        /// items.
        /// </summary>
        public Set(IEqualityComparer<T> equalityComparer)
        {
            _dictionary = new Dictionary<T, bool>();
        }

        /// <summary>
        /// Creates set with the default equality comparer
        /// for the item type initialized with the items in the collection
        /// </summary>
        public Set(IEnumerable<T> collection)
            : this()
        {
            Add(collection);
        }

        /// <summary>
        /// Creates set with the specified equality comparer
        /// to compare items initialized with the items in the collection
        /// </summary>
        public Set(IEnumerable<T> collection, IEqualityComparer<T> equalityComparer)
            : this(equalityComparer)
        {
            Add(collection);
        }

        /// <summary>
        /// Adds all the elements in the collection to this set
        /// </summary>
        /// <param name="collection">collection of items to add</param>
        public void Add(IEnumerable<T> collection)
        {
            foreach (T item in collection)
                Add(item);
        }

        #region ICollection<T> Members

        /// <summary>
        /// Add the item to the set
        /// </summary>
        /// <param name="item">item to add</param>
        public void Add(T item)
        {
            _dictionary[item] = true;
        }

        /// <summary>
        /// Remove all items from the set
        /// </summary>
        public void Clear()
        {
            _dictionary.Clear();
        }

        /// <summary>
        /// Checks to see if the item exists in the set
        /// </summary>
        /// <param name="item">the item to check</param>
        /// <returns>true if in set</returns>
        public bool Contains(T item)
        {
            return _dictionary.ContainsKey(item);
        }

        /// <summary>
        /// Returns true if this set contains any items in the set to check
        /// </summary>
        /// <param name="set">items to check against</param>
        public bool ContainsAny(IEnumerable<T> set)
        {
            foreach (T item in set)
                if (Contains(item))
                    return true;

            return false;
        }

        /// <summary>
        /// Returns true if this set contains all of the items in the set to check
        /// </summary>
        /// <param name="set">items to check against</param>
        public bool ContainsAll(IEnumerable<T> set)
        {
            foreach (T item in set)
                if (!Contains(item))
                    return false;
            return true;
        }

        /// <summary>
        /// Copies the items in the set to an array
        /// </summary>
        /// <param name="array">the array instance to copy to</param>
        /// <param name="arrayIndex">the index to start copying at</param>
        public void CopyTo(T[] array, int arrayIndex)
        {
            _dictionary.Keys.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Returns the number of items in the set
        /// </summary>
        public int Count
        {
            get { return _dictionary.Count; }
        }


        public bool IsReadOnly
        {
            get { return false; }
        }

        /// <summary>
        /// Removes an item from the set
        /// </summary>
        /// <param name="item">the item to remove</param>
        /// <returns>true if the item was in the set</returns>
        public bool Remove(T item)
        {
            return _dictionary.Remove(item);
        }

        #endregion

        /// <summary>
        /// Removes all the items in the collection from this set
        /// </summary>
        /// <param name="items">items to remove</param>
        public void Remove(IEnumerable<T> items)
        {
            foreach(T item in items)
                Remove(item);
        }
        /// <summary>
        /// Creates a new set that contains all the items in the current set and
        /// all the items in the sets to union
        /// </summary>
        /// <param name="toUnion"></param>
        /// <returns></returns>
        public ISet<T> Union(IEnumerable<T> collection)
        {
            Set<T> result = new Set<T>();
            result.Add(collection);
            return result;
        }

        /// <summary>
        /// Returns a set that is the intersection of this set and the other set, i.e. a
        /// set containing the items that exist in both sets
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public ISet<T> Intersect(ICollection<T> set)
        {
            Set<T> result = new Set<T>();
            ICollection<T> smallest = this;
            ICollection<T> biggest = set;
            if (set.Count < this.Count)
            {
                smallest = set;
                biggest = this;
            }
            foreach (T item in smallest)
            {
                if (biggest.Contains(item))
                    result.Add(item);
            }
            return result;
        }

        /// <summary>
        /// Returns a new set that contains the items in this set minus those
        /// in the set to exclude
        /// </summary>
        /// <param name="toExclude">items to exclude</param>
        /// <returns></returns>
        public ISet<T> Exclude(IEnumerable<T> toExclude)
        {
            Set<T> result = new Set<T>(this);
            result.Remove(toExclude);
            return result;
        }
        #region IEnumerable<T> Members

        public IEnumerator<T> GetEnumerator()
        {
            return _dictionary.Keys.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion
    }
}

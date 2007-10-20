using System;
using System.Collections.Generic;
namespace Mirage.Core.Util
{

    /// <summary>
    /// A collection of items that contains no duplicate items.
    /// </summary>
    /// <typeparam name="T">the item type</typeparam>
    public interface ISet<T> : ICollection<T>
    {
        /// <summary>
        /// Copies the contents to an array
        /// </summary>
        /// <returns>an array</returns>
        T[] ToArray();

        /// <summary>
        /// Adds all the items in the collection to this set
        /// </summary>
        /// <param name="collection"></param>
        void Add(IEnumerable<T> collection);

        /// <summary>
        /// Removes all of the specified items from this set
        /// </summary>
        /// <param name="items">the items to remove</param>
        void Remove(IEnumerable<T> items);

        /// <summary>
        /// Returns true if this set contains any items in the set to check
        /// </summary>
        /// <param name="set">items to check against</param>
        bool ContainsAny(IEnumerable<T> set);

        /// <summary>
        /// Returns true if this set contains all of the items in the set to check
        /// </summary>
        /// <param name="set">items to check against</param>
        bool ContainsAll(IEnumerable<T> set);

        /// <summary>
        /// Creates a new set of the union of this set and the collections in the toUnion params
        /// </summary>
        /// <param name="collection">collections to union</param>
        /// <returns>a new set</returns>
        ISet<T> Union(IEnumerable<T> collection);

        /// <summary>
        /// Creates a new set containing the items that are contained in both this set and the specified collection
        /// </summary>
        /// <param name="collection">the collection of items to intersect with</param>
        /// <returns>a new set</returns>
        ISet<T> Intersect(ICollection<T> collection);

        /// <summary>
        /// Creates a new set containing the items in the current set minus those in the collection
        /// </summary>
        /// <param name="toExclude">collection of items to exclude</param>
        /// <returns>a new set</returns>
        ISet<T> Exclude(IEnumerable<T> toExclude);
    }
}

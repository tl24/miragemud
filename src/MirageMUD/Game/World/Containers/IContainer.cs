using System;
using System.Collections;
using System.Collections.Generic;

namespace Mirage.Game.World.Containers
{

    /// <summary>
    /// An interface implemented by an object that can contain other objects.  This is a container
    /// in a physical sense, not OO-programming sense.  i.e. players can be contained in rooms, but
    /// rooms are generally not contained in an area, even though the area may have a collection of rooms.
    /// The rooms are part of the area, not contained in it.
    /// </summary>
    public interface IContainer : IEnumerable
    {
        /// <summary>
        /// Adds an item to this container
        /// </summary>
        /// <param name="item">the item to add</param>
        /// <exception cref="Mirage.Game.World.Containers.ContainerAddException">When the item cannot be added to the container</exception>
        void Add(object item);

        /// <summary>
        /// Removes the item from this container
        /// </summary>
        /// <param name="item">the item to be removed</param>
        void Remove(object item);

        /// <summary>
        /// Checks to see if the given item resides in this container.
        /// </summary>
        /// <param name="item">the item to search for</param>
        /// <returns>true if the item is inside this container</returns>
        bool Contains(object item);

        /// <summary>
        /// Checks to see if a given item can be added to this container.
        /// </summary>
        /// <param name="item">the item to check</param>
        /// <returns>true if the item can be added</returns>
        bool CanAdd(object item);

        /// <summary>
        /// Gets the number of items in the container
        /// </summary>
        int Count { get; }
    }
}

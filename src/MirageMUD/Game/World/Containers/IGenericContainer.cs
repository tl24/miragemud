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
    public interface IContainer<T> : IContainer, IEnumerable<T>
    {
        /// <summary>
        /// Adds an item to this container
        /// </summary>
        /// <param name="item">the item to add</param>
        /// <exception cref="Mirage.Game.World.Containers.ContainerAddException">When the item cannot be added to the container</exception>
        void Add(T item);

        /// <summary>
        /// Removes the item from this container
        /// </summary>
        /// <param name="item">the item to be removed</param>
        void Remove(T item);

        /// <summary>
        /// Checks to see if the given item resides in this container.
        /// </summary>
        /// <param name="item">the item to search for</param>
        /// <returns>true if the item is inside this container</returns>
        bool Contains(T item);
    }
}

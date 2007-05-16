using System;
using System.Collections.Generic;
using System.Text;

namespace Shoop.Data
{

    /// <summary>
    /// An interface implemented by an object that can contain other objects.  This is a container
    /// in a physical sense, not OO-programming sense.  i.e. players can be contained in rooms, but
    /// rooms are generally not contained in an area, even though the area may have a collection of rooms.
    /// The rooms are part of the area, not contained in it.
    /// </summary>
    public interface IContainer
    {
        /// <summary>
        /// Adds an item to this container
        /// </summary>
        /// <param name="item">the item to add</param>
        /// <exception cref="Shoop.Data.ContainerAddException">When the item cannot be added to the container</exception>
        void Add(IContainable item);

        /// <summary>
        /// Removes the item from this container
        /// </summary>
        /// <param name="item">the item to be removed</param>
        void Remove(IContainable item);

        /// <summary>
        /// Checks to see if the given item resides in this container.
        /// </summary>
        /// <param name="item">the item to search for</param>
        /// <returns>true if the item is inside this container</returns>
        bool Contains(IContainable item);

        /// <summary>
        /// Checks to see if this container can hold items of the given type.  This does not 
        /// guarantee that a given instance of the type will be able to be added to the container.
        /// Call CanAdd to see if a particular instance can be added.
        /// </summary>
        /// <param name="item">the type to check</param>
        /// <returns>true if the container can hold this type</returns>
        bool CanContain(Type item);

        /// <summary>
        /// Checks to see if a given item can be added to this container.
        /// </summary>
        /// <param name="item">the item to check</param>
        /// <returns>true if the item can be added</returns>
        bool CanAdd(IContainable item);
    }
}

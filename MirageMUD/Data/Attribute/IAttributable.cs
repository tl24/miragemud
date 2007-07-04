using System;
using System.Collections.Generic;
using System.Text;

namespace Mirage.Data.Attribute
{
    /// <summary>
    /// Interface for an object that supports adding and removing attributes at runtime.
    /// </summary>
    public interface IAttributable : ISupportAttribute
    {
        /// <summary>
        /// Adds an attribute to the object
        /// </summary>
        /// <param name="attribute">the attribute object to add</param>
        /// <returns>true if the attribute was added</returns>
        bool AddAttribute(object attribute);

        /// <summary>
        /// Removes an attribute from the object
        /// </summary>
        /// <param name="attribute">the type of attribute to remove</param>
        /// <returns>true if attribute type successfully removed</returns>
        bool RemoveAttribute(Type t);
    }
}

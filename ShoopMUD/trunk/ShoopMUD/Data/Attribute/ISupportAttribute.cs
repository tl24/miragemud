using System;
using System.Collections.Generic;
using System.Text;

namespace Shoop.Data.Attribute
{
    /// <summary>
    /// An interface that supports querying an object for supported attributes.
    /// </summary>
    public interface ISupportAttribute
    {
        /// <summary>
        /// Checks to see if the object has the given attribute
        /// </summary>
        /// <param name="t">the type of the attribute</param>
        /// <returns>true if object has attribute</returns>
        bool HasAttribute(Type t);

        /// <summary>
        /// Trys to get the attribute specified by <paramref name="t"/>.
        /// If the object does not support the attribute an exception is thrown.
        /// </summary>
        /// <param name="t">the type of the attribute</param>
        /// <returns>an object implementing the desired attribute</returns>
        object GetAttribute(Type t);


        /// <summary>
        /// Trys to get the attribute specified by <paramref name="t"/>.
        /// If the object does not support the attribute null is returned.
        /// </summary>
        /// <param name="t">the type of the attribute</param>
        /// <returns>an object implementing the desired attribute or null</returns>
        object TryGetAttribute(Type t);
    }
}

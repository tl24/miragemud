using System;
using System.Collections.Generic;
using System.Text;

namespace Mirage.Core.Data
{

    /// <summary>
    /// Interface implemented by an object that can reside in a container
    /// </summary>
    public interface IContainable
    {
        /// <summary>
        /// Sets or gets a reference to the items container if it is contained by
        /// another object
        /// </summary>
        IContainer Container { get; set; }

        /// <summary>
        /// Returns true if this object can be contained inside the given container
        /// </summary>
        /// <param name="container">the container the object is being added to</param>
        /// <returns>true if the object can be contained inside this container</returns>
        bool CanBeContainedBy(IContainer container);
    }
}

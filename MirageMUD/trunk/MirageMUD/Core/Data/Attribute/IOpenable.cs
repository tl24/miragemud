using System;
using System.Collections.Generic;
using System.Text;

namespace Mirage.Core.Data.Attribute
{
    /// <summary>
    /// Attribute for an object that can be opened and closed.
    /// </summary>
    public interface IOpenable
    {
        /// <summary>
        /// Checks to see if the object is open
        /// </summary>
        /// <returns>true if it is opened, false if it is closed</returns>
        bool IsOpen();

        /// <summary>
        /// Opens the object.  This will thrown an OperationNotSupportedException
        /// if the object is already open.
        /// </summary>
        void Open();

        /// <summary>
        /// Closes the object.
        /// </summary>
        void Close();
    }
}

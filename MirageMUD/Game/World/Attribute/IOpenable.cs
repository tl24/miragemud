using System;
using System.Collections.Generic;
using System.Text;

namespace Mirage.Game.World.Attribute
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
        bool Opened { get; }

        /// <summary>
        /// Opens the object.
        /// </summary>
        /// <exception cref="ValidationException">if the object is already opened or can't be opened.</exception>
        void Open();

        /// <summary>
        /// Closes the object.
        /// </summary>
        /// <exception cref="ValidationException">if the object is already closed or can't be closed.</exception>
        void Close();
    }
}

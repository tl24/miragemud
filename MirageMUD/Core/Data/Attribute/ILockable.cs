using System;
using System.Collections.Generic;
using System.Text;
using Mirage.Core.Data.Query;

namespace Mirage.Core.Data.Attribute
{
    /// <summary>
    /// Interface for an object that can be locked.
    /// </summary>
    public interface ILockable : IOpenable
    {
        /// <summary>
        /// Checks to see if the object is locked
        /// </summary>
        /// <returns>true if locked</returns>
        bool IsLocked();

        /// <summary>
        /// Locks the object.  The object must first be closed.
        /// The caller should check to see if the actor locking the object
        /// has the specified key by calling IsKey with the canidate key object.
        /// </summary>
        void Lock();

        /// <summary>
        /// Unlocks the object.  The object must first be locked.
        /// The caller should check to see if the actor unlocking the object
        /// has the specified key by calling IsKey with the canidate key object.
        /// </summary>
        void Unlock();

        /// <summary>
        /// Returns the uri for the key object that locks/unlocks this object.
        /// </summary>
        string Key { get; set; }

        /// <summary>
        /// Checks to see if the given object is the key to this lock
        /// </summary>
        /// <param name="keyObj"></param>
        /// <returns>true if this is the key that unlocks the object</returns>
        bool IsKey(IUri keyObj);
    }
}

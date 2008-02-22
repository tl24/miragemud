using System;
using System.Collections.Generic;
using System.Text;
using Mirage.Core.Data.Query;
using Mirage.Core.Communication;
using Mirage.Core.Util;

namespace Mirage.Core.Data.Attribute
{
    /// <summary>
    /// LockableAttribute is applied to an object so that it can be locked.
    /// </summary>
    public class LockableAttribute : OpenableAttribute, ILockable
    {
        protected bool _isLocked;
        protected string _key;

        /// <summary>
        /// Constructs the lockable object with no target
        /// </summary>
        public LockableAttribute()
            : base()
        {
            _isLocked = true;
            _isOpen = false;
        }

        /// <summary>
        /// constructs the lockable object with the specified target and initial state parameters.
        /// </summary>
        /// <param name="target">the target that the lockable attribute is applied to</param>
        /// <param name="isOpen">initial open flag</param>
        /// <param name="isLocked">initial lock flag</param>
        /// <param name="key">the uri that identifies the key</param>
        public LockableAttribute(IAttributable target, bool isOpen, bool isLocked, string key)
            : base(target, isOpen)
        {
            if (isLocked && isOpen)
            {
                throw new ArgumentException("Object can not be locked and open at the same time.");
            }
            _isLocked = isLocked;
            _key = key;
        }

        /// <summary>
        /// Constructs the lockable object with the given target and key with the object defaulting
        /// to closed and locked states.
        /// </summary>
        /// <param name="target">the target that the lockable attribute is applied to</param>
        /// <param name="key">the uri that identifies the key</param>
        public LockableAttribute(IAttributable target, string key)
            : this(target, false, true, key)
        {
        }

        #region ILockable Members

        /// <summary>
        /// Indicates whether the object is currently locked
        /// </summary>
        public bool Locked
        {
            get { return _isLocked; }
        }


        public void Lock()
        {
            if (Opened)
                throw new ValidationException("common.error.CantLockWhenOpen");
            
            if (Locked)
                throw new ValidationException("common.error.ObjectAlreadyLocked");

            _isLocked = true;
        }

        public void Lock(IUri key)
        {
            ValidateKey(key);
            Lock();
        }

        public void Unlock()
        {
            if (!Locked)
                throw new ValidationException("common.error.ObjectAlreadyUnlocked");

            _isLocked = false;
        }

        public void Unlock(IUri key)
        {
            ValidateKey(key);
            Unlock();
        }

        /// <summary>
        /// Checks to see if the key is the key for this lock.  If not, it will
        /// throw a validation exception
        /// </summary>
        /// <param name="key">the key object to compare to this lock's key</param>
        /// <exception cref="ValidationException">if the key is not the right key</exception>
        private void ValidateKey(IUri key)
        {
            if (!IsKey(key))
                throw new ValidationException("common.error.WrongKey");
        }

        /// <summary>
        /// Gets or Sets the uri for the key that opens this object
        /// </summary>
        public string Key
        {
            get { return _key; }
            set { _key = value; }
        }

        /// <summary>
        /// Check to see if the object is locked before opening it
        /// </summary>
        public override void Open()
        {
            if (Locked)
                throw new ValidationException("common.error.ObjectLocked");
            base.Open();
        }

        /// <summary>
        /// Checks to see if the given object can unlock the object
        /// </summary>
        /// <param name="keyObj"></param>
        /// <returns></returns>
        public bool IsKey(IUri keyObj)
        {
            if (_key == null || _key.Length == 0)
                return keyObj == null;

            return keyObj.FullUri.Equals(_key, StringComparison.CurrentCultureIgnoreCase);
        }

        public override string ToString()
        {
            if (Key != string.Empty)
                return "Lockable(" + Key + ")";
            else
                return "Lockable";
        }
        #endregion

        #region static helper methods

        /// <summary>
        /// If the target implements ILockable, check to see if it is locked,
        /// otherwise return true if its not lockable
        /// </summary>
        /// <param name="target">the target object to check</param>
        /// <returns>true if locked, otherwise false</returns>
        public static bool IsLocked(object target)
        {
            ILockable l = TypeSupports.TryCastAs<ILockable>(target);
            return l == null ? false : l.Locked;
        }

        #endregion
    }
}

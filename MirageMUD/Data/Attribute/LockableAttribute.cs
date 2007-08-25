using System;
using System.Collections.Generic;
using System.Text;
using Mirage.Data.Query;

namespace Mirage.Data.Attribute
{
    public class LockableAttribute : OpenableAttribute, ILockable
    {
        protected bool _isLocked;
        protected string _key;

        public LockableAttribute()
            : base()
        {
            _isLocked = true;
            _isOpen = false;
        }

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

        public LockableAttribute(IAttributable target, string key)
            : this(target, false, true, key)
        {
        }

        #region ILockable Members

        public bool IsLocked()
        {
            return _isLocked;
        }

        public void Lock()
        {
            if (IsOpen())
            {
                throw new InvalidOperationException("Object can not be locked when it is open.");
            }
            else if (IsLocked())
            {
                throw new InvalidOperationException("Object is already locked.");
            }

            _isLocked = true;
        }

        public void Unlock()
        {
            if (!IsLocked())
            {
                throw new InvalidOperationException("Object is already unlocked.");
            }
            _isLocked = false;
        }

        public string Key
        {
            get
            {
                return _key;
            }
            set
            {
                _key = value;
            }
        }

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
    }
}

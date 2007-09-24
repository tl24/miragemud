using System;
using System.Collections.Generic;
using System.Text;

namespace Mirage.Core.Data.Attribute
{
    public class OpenableAttribute : BaseAttribute, IOpenable
    {
        protected bool _isOpen;

        public OpenableAttribute(IAttributable target)
            : base(target)
        {
        }

        public OpenableAttribute()
            : base()
        {
        }

        public OpenableAttribute(IAttributable target, bool isOpen)
            : base(target)
        {
            _isOpen = isOpen;
        }

        #region IOpenable Members

        public bool IsOpen()
        {
            return _isOpen;
        }

        public void Open()
        {
            if (IsOpen())
            {
                throw new InvalidOperationException("Object is already open.");
            }
            else
            {
                _isOpen = true;
            }
        }

        public void Close()
        {
            if (!IsOpen())
            {
                throw new InvalidOperationException("Object is already closed.");
            }
            else
            {
                _isOpen = false;
            }
        }

        #endregion

        /// <summary>
        /// Checks to see if the object is open, if the the object does not
        /// support IOpenable interace, then true is returned, otherwise the
        /// IsOpen property is checked.
        /// </summary>
        /// <param name="target">the object to check</param>
        /// <returns>true if open or not supported, false otherwise</returns>
        public static bool IsOpen(object target)
        {
            if (target is IAttributable)
            {
                IOpenable o = (IOpenable) ((IAttributable)target).TryGetAttribute(typeof(IOpenable));
                return o == null ? true : o.IsOpen();
            }
            return true;
        }
    }
}

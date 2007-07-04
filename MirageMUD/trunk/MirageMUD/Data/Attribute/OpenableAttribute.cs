using System;
using System.Collections.Generic;
using System.Text;

namespace Shoop.Data.Attribute
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
    }
}

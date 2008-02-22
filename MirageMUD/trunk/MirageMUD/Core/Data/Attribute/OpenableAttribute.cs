using System;
using System.Collections.Generic;
using System.Text;
using Mirage.Core.Communication;
using Mirage.Core.Util;

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

        public bool Opened
        {
            get { return _isOpen; }
        }

        public virtual void Open()
        {
            if (Opened)
            {
                throw new ValidationException("common.error.ObjectAlreadyOpen");
            }
            else
            {
                _isOpen = true;
            }
        }

        public virtual void Close()
        {
            if (!Opened)
            {
                throw new ValidationException("common.error.ObjectAlreadyClosed");
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
            IOpenable o = TypeSupports.TryCastAs<IOpenable>(target);
            return o == null ? true : o.Opened;
        }

        /// <summary>
        /// Checks to see if the given object implements IOpenable and attempts to open it.
        /// If it doesn't implement openable, an exception is thrown.
        /// </summary>
        /// <param name="target">the object to open</param>
        /// <exception cref="ValidationException">if the object does not support IOpenable or the object can't be opened</exception>
        public static void Open(object target)
        {
            IOpenable o = TypeSupports.TryCastAs<IOpenable>(target);
            if (o != null)
                o.Open();
            else
                throw new ValidationException("common.error.NotOpenable");
        }

        /// <summary>
        /// Checks to see if the given object implements IOpenable and attempts to close it.
        /// If it doesn't implement IOpenable, an exception is thrown.
        /// </summary>
        /// <param name="target">the object to close</param>
        /// <exception cref="ValidationException">if the object does not support IOpenable or the object can't be closed</exception>
        public static void Close(object target)
        {
            IOpenable o = TypeSupports.TryCastAs<IOpenable>(target);
            if (o != null)
                o.Close();
            else
                throw new ValidationException("common.error.NotCloseable");
        }
    }
}

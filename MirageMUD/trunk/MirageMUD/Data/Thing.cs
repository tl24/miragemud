using System;
using System.Collections.Generic;
using System.Text;
using Mirage.Data.Attribute;
using System.Collections;

namespace Mirage.Data
{

    /// <summary>
    /// Base class for most mud objects
    /// </summary>
    public class Thing : ISupportAttribute
    {
        protected ArrayList _attributes;

        public Thing()
        {
            _attributes = new ArrayList();
        }
        #region ISupportAttribute Members

        public bool HasAttribute(Type t)
        {
            return (TryGetAttribute(t) != null);
        }

        public object GetAttribute(Type t)
        {
            object attr = TryGetAttribute(t);
            if (attr == null)
            {
                throw new AttributeNotSupportedException();
            }
            return attr;
        }

        public object TryGetAttribute(Type t)
        {
            if (t.IsAssignableFrom(this.GetType())) {
                return this;
            }

            foreach (object o in _attributes)
            {
                if (t.IsAssignableFrom(o.GetType()))
                {
                    return o;
                }
            }
            return null;
        }

        #endregion
    }
}

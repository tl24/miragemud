using System;
using System.Collections.Generic;
using System.Text;

namespace Shoop.Data.Attribute
{
    class AttributeNotSupportedException : ApplicationException
    {
        public AttributeNotSupportedException()
            : base()
        {
        }

        public AttributeNotSupportedException(string message)
            : base(message)
        {
        }

        public AttributeNotSupportedException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
        
    }
}

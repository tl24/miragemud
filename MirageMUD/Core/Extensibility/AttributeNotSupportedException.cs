using System;

namespace Mirage.Core.Extensibility
{
    public class AttributeNotSupportedException : Exception
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

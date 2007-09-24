using System;
using System.Collections.Generic;
using System.Text;

namespace Mirage.Core.Data
{
    public class ObjectNotFoundException : System.ApplicationException
    {
        public ObjectNotFoundException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public ObjectNotFoundException(string message)
            : base(message)
        {
        }

        public ObjectNotFoundException()
            : base()
        {
        }

    }
}

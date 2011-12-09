using System;

namespace Mirage.Game.World
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

using System;

namespace Mirage.Game.World.Containers
{
    /// <summary>
    /// Exception thrown when an object can't be added to a container
    /// </summary>
    public class ContainerAddException : ApplicationException
    {
        private object _container;
        private object _item;

        public ContainerAddException() : base() { }

        public ContainerAddException(string message) : base(message) { }

        public ContainerAddException(string message, Exception innerException) : base(message, innerException) { }

        public ContainerAddException(string message, object container, object item)
            : base(message)
        {
            this._container = container;
            this._item = item;
        }

        public ContainerAddException(string message, Exception innerException, object container, object item)
            : base(message, innerException)
        {
            this._container = container;
            this._item = item;
        }

        public object Container
        {
            get { return this._container; }
            set { this._container = value; }
        }

        public object Item
        {
            get { return this._item; }
            set { this._item = value; }
        }

        
    }
}

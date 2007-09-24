using System;
using System.Collections.Generic;
using System.Text;

namespace Mirage.Core.Data
{
    /// <summary>
    /// Exception thrown when an object can't be added to a container
    /// </summary>
    public class ContainerAddException : ApplicationException
    {
        private IContainer _container;
        private IContainable _item;

        public ContainerAddException() : base() { }

        public ContainerAddException(string message) : base(message) { }

        public ContainerAddException(string message, Exception innerException) : base(message, innerException) { }

        public ContainerAddException(string message, IContainer container, IContainable item) : base(message) {
            this._container = container;
            this._item = item;
        }

        public ContainerAddException(string message, Exception innerException, IContainer container, IContainable item) : base(message, innerException) {
            this._container = container;
            this._item = item;
        }

        public IContainer Container
        {
            get { return this._container; }
            set { this._container = value; }
        }

        public IContainable Item
        {
            get { return this._item; }
            set { this._item = value; }
        }

        
    }
}

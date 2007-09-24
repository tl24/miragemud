using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace Mirage.Core.Communication
{
    /// <summary>
    /// Data message for sending a child collection of an object
    /// (when the child property is lazy loaded)
    /// </summary>
    public class ChildItemsMessage : DataMessage
    {
        private ICollection _items;

        public ChildItemsMessage()
            : base()
        {
        }

        public ChildItemsMessage(Uri Namespace, string name)
            : base(Namespace, name)
        {
        }

        public ChildItemsMessage(Uri Namespace, string name, object parent, ICollection items)
            : base(Namespace, name, parent)
        {
            this.Items = items;
        }

        public new object Parent
        {
            get { return Data; }
            set { Data = value; }
        }

        public ICollection Items
        {
            get { return (ICollection)_items; }
            set { _items = value; }
        }
    }
}

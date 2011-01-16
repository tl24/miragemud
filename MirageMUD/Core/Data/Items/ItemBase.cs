using System;
using System.Collections.Generic;
using System.Text;
using Mirage.Core.Data;
using Mirage.Core.Data.Containers;

namespace Mirage.Core.Data.Items
{
    public class ItemBase : ViewableBase, IContainable
    {
        private IContainer _container;

        public ItemBase()
            : base()
        {
        }

        #region IContainable Members

        public IContainer Container
        {
            get
            {
                return _container;
            }
            set
            {
                _container = value;
            }
        }

        public bool CanBeContainedBy(IContainer container)
        {
            return (container is Room) || (container is Living) || (container is ItemBase);
        }

        #endregion

    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace Mirage.Core.Data
{
    public class Containers
    {
        public static bool TryTransfer(IContainable item, IContainer newContainer)
        {
            if (newContainer.CanAdd(item))
            {
                Transfer(item, newContainer);
                return true;
            }
            else
            {
                return false;
            }
        }

        public static void Transfer(IContainable item, IContainer newContainer)
        {
            IContainer oldContainer = item.Container;
            oldContainer.Remove(item);
            newContainer.Add(item);
            if (item.Container != newContainer)
                item.Container = newContainer;

        }
    }
}
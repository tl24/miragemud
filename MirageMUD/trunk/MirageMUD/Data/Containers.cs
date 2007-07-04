using System;
using System.Collections.Generic;
using System.Text;

namespace Mirage.Data
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
            newContainer.Add(item);
            oldContainer.Remove(item);
            if (item.Container != newContainer)
                item.Container = newContainer;

        }
    }
}

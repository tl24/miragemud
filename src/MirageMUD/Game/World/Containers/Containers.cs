
namespace Mirage.Game.World.Containers
{
    public class ContainerUtils
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
            if (oldContainer != null)
                oldContainer.Remove(item);

            newContainer.Add(item);
            if (item.Container != newContainer)
                item.Container = newContainer;

        }

        public static bool TryTransfer<T>(T item, IContainer<T> newContainer)
        {
            Transfer(item, newContainer);
            return true;
        }

        public static void Transfer<T>(T item, IContainer<T> newContainer)
        {
            IContainable containable = item as IContainable;
            if (containable != null)
            {
                IContainer oldContainer = containable.Container;
                if (oldContainer != null)
                    oldContainer.Remove(item);
            }
            newContainer.Add(item);
            if (containable != null && containable.Container != newContainer)
            {
                containable.Container = newContainer;
            }
        }

    }
}

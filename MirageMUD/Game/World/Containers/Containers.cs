
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
    }
}

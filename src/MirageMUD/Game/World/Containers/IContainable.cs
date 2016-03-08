
namespace Mirage.Game.World.Containers
{

    /// <summary>
    /// Interface implemented by an object that can reside in a container
    /// </summary>
    public interface IContainable
    {
        /// <summary>
        /// Sets or gets a reference to the items container if it is contained by
        /// another object
        /// </summary>
        IContainer Container { get; set; }
    }
}

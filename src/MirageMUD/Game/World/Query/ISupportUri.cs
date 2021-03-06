
namespace Mirage.Game.World.Query
{
    public interface ISupportUri
    {
        /// <summary>
        /// Returns the Uri for this object.
        /// </summary>
        string Uri { get; }

        /// <summary>
        /// Gets the full Uri for this object which includes
        /// the full Uri of the Parent.
        /// </summary>
        string FullUri { get; }
    }
}

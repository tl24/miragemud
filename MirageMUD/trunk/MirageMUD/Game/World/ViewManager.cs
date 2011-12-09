
namespace Mirage.Game.World
{
    /// <summary>
    /// Default view manager implementation.  For now, no logic is performed, 
    /// the field requested is just returned unaltered.
    /// </summary>
    public class ViewManager : IViewManager
    {
        #region IViewManager Members

        public string GetTitle(Living observer, IViewable target)
        {
            return target.Title;
        }

        public string GetShort(Living observer, IViewable target)
        {
            return target.ShortDescription;
        }

        public string GetLong(Living observer, IViewable target)
        {
            return target.LongDescription;
        }

        public VisiblityType GetVisibility(Living observer, IViewable target)
        {
            return VisiblityType.Visible;
        }

        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace Mirage.Stock.Data
{
    /// <summary>
    /// Default view manager implementation.  For now, no logic is performed, 
    /// the field requested is just returned unaltered.
    /// </summary>
    public class ViewManager : IViewManager
    {
        #region IViewManager Members

        public string GetTitle(Living observer, Living target)
        {
            return target.Title;
        }

        public string GetShort(Living observer, Living target)
        {
            return target.ShortDescription;
        }

        public string GetLong(Living observer, Living target)
        {
            return target.LongDescription;
        }

        public VisiblityType GetVisibility(Living observer, Living target)
        {
            return VisiblityType.Visible;
        }

        #endregion
    }
}

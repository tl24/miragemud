using System;
using System.Collections.Generic;
using System.Text;

namespace Mirage.Core.Data
{
    public enum VisiblityType
    {
        Visible,
        NotVisible
    }

    /// <summary>
    /// Manages interactions between players, mobiles and objects and
    /// answers questions such as Can player A see player B?
    /// </summary>
    public interface IViewManager
    {
        /// <summary>
        /// Gets the title for a target from the observer's perspective.  If the observer
        /// is unable to see the target, an anoymous string such as "someone" will be returned.
        /// </summary>
        /// <param name="observer">the mobile or player that is looking</param>
        /// <param name="target">the target mobile or player</param>
        /// <returns>title</returns>
        string GetTitle(Living observer, IViewable target);

        /// <summary>
        /// Gets the short description for a target from the observer's perspective.  If the observer
        /// is unable to see the target, an anoymous string such as "someone" will be returned.
        /// </summary>
        /// <param name="observer">the mobile or player that is looking</param>
        /// <param name="target">the target mobile or player</param>
        /// <returns>title</returns>
        string GetShort(Living observer, IViewable target);

        /// <summary>
        /// Gets the long description for a target from the observer's perspective.  If the observer
        /// is unable to see the target, an anoymous string such as "someone" will be returned.
        /// </summary>
        /// <param name="observer">the mobile or player that is looking</param>
        /// <param name="target">the target mobile or player</param>
        /// <returns>title</returns>
        string GetLong(Living observer, IViewable target);

        /// <summary>
        /// Returns the visibility of the target to the observer
        /// </summary>
        /// <param name="observer">observer</param>
        /// <param name="target">target</param>
        /// <returns></returns>
        VisiblityType GetVisibility(Living observer, IViewable target);
    }
}

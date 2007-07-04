using System;
using System.Collections.Generic;
using System.Text;

namespace Shoop.Data.Query
{
    public interface IUri
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

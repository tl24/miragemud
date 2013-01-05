using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mirage.Game.World.Query
{
    [Flags]
    public enum QueryHints
    {
        /// <summary>
        /// Items in the collection are unique by Uri
        /// </summary>
        UniqueItems,
        /// <summary>
        /// Items are sorted, the query manager can use this to optimize its search
        /// </summary>
        Sorted,
        /// <summary>
        /// Collection defaults to a partial match if the match type is not specified on the
        /// query
        /// </summary>
        DefaultPartialMatch,
        /// <summary>
        /// Indicates that the keys for the dictionary are the Uris of the items
        /// </summary>
        UriKeyedDictionary
    }

}

using System;
using System.Collections.Generic;
using System.Text;

namespace Shoop.Data.Query
{
    public class QueryMatcher
    {

        public static QueryMatcher getMatcher(ObjectQuery query)
        {
            return new QueryMatcher(query);
        }

        private ObjectQuery _query;
        
        private QueryMatcher(ObjectQuery query)
        {
            this._query = query;
        }

        /// <summary>
        /// Checks the object against the given query to see if there is a match
        /// </summary>
        /// <param name="obj">the object to match</param>
        /// <returns>true if the object matches</returns>
        public bool IsMatch(IQueryable obj) {
            // simple match for now
            if ((_query.Flags & QueryFlags.IsExact) == QueryFlags.IsExact)
            {
                if (!(obj.URI == _query.UriName))
                    return false;
            }
            else if ((_query.Flags ^ (QueryFlags.Wildcard | QueryFlags.All)) != 0)
            {                
                if (!obj.URI.StartsWith(_query.UriName.Substring(0, _query.UriName.Length - 1), StringComparison.CurrentCultureIgnoreCase))
                    return false;
            }
            if ((_query.Flags & QueryFlags.TypeMatch) == QueryFlags.TypeMatch)
            {
                if (obj.GetType().Name != _query.TypeName
                    && obj.GetType().FullName != _query.TypeName)
                {
                    return false;
                }
            }
            return true;
        }

    }
}

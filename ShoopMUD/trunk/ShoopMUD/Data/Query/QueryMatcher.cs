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
            QueryMatchType matchType = GetMatchType(QueryMatchType.Default);
            // simple match for now
            if (matchType == QueryMatchType.Exact)
            {
                if (!(obj.Uri == _query.UriName))
                    return false;
            }
            else if (matchType == QueryMatchType.Partial)
            {                
                if (!obj.Uri.StartsWith(_query.UriName, StringComparison.CurrentCultureIgnoreCase))
                    return false;
            }
            if (_query.TypeName != null)
            {
                if (obj.GetType().Name != _query.TypeName
                    && obj.GetType().FullName != _query.TypeName)
                {
                    return false;
                }
            }
            return true;
        }

        private QueryMatchType GetMatchType(QueryMatchType desired)
        {
            QueryMatchType result = desired;
            if (result == QueryMatchType.Default)
            {
                result = _query.MatchType;
            }
            if (result == QueryMatchType.Default)
            {
                result = QueryMatchType.Exact;
            }
            return result;
        }
    }
}

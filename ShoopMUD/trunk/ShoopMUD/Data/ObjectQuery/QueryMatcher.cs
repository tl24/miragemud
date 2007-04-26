using System;
using System.Collections.Generic;
using System.Text;

namespace Shoop.Data.Query
{
    public class QueryMatcher<T> where T : IQueryable
    {
        [Flags]
        private enum QueryFlags
        {
            IsExact,
            All,
            Wildcard,
            TypeMatch
        }

        public static QueryMatcher<T> getMatcher(ObjectQuery query)
        {
            return new QueryMatcher<T>(query);
        }

        private ObjectQuery _query;
        private QueryFlags _flags;
        
        private QueryMatcher(ObjectQuery query)
        {
            this._query = query;
            // parse the flag values
            _flags = 0;
            _flags |= _query.TypeName != null ? QueryFlags.TypeMatch : 0;
            if (_query.UriName.Contains("*"))
            {
                _flags |= QueryFlags.Wildcard;
                _flags |= _query.UriName == "*" ? QueryFlags.All : 0;
            }
            else
            {
                _flags |= QueryFlags.IsExact;
            }
        }

        public bool IsMatch(T obj) {
            // simple match for now
            if ((_flags & QueryFlags.IsExact) != 0)
            {
                if (!(obj.URI == _query.UriName))
                    return false;
            }
            else if ((_flags ^ (QueryFlags.Wildcard | QueryFlags.All)) != 0)
            {
                if (!obj.URI.StartsWith(_query.UriName.Substring(0, _query.UriName.Length - 1), StringComparison.CurrentCultureIgnoreCase))
                    return false;
            }
            if ((_flags | QueryFlags.TypeMatch) != 0)
            {
                if (typeof(T).Name != _query.TypeName
                    && typeof(T).FullName != _query.TypeName)
                {
                    return false;
                }
            }
            return true;
        }

    }
}

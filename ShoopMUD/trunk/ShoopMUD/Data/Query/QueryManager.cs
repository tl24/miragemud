using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace Shoop.Data.Query
{

    /// <summary>
    /// Refactor this class so that it can search objects that are not IQueryable
    /// </summary>
    public class QueryManager
    {
        public QueryManager()
        {
        }

        public static QueryManager GetInstance()
        {
            return new QueryManager();
        }

        /// <summary>
        /// Finds the first object matching the given query
        /// </summary>
        /// <param name="query">the uri query</param>
        /// <returns>the first matching object</returns>
        public object Find(object searched, ObjectQuery query)
        {
            if (query.IsAbsolute)
            {
                searched = GlobalLists.GetInstance();
            }
            if (searched is IQueryable)
            {
                return ((IQueryable)searched).Find(query);
            } 
            else if (searched is IUriContainer && query.MatchType == QueryMatchType.Exact)
            {
                IUriContainer container = searched as IUriContainer;
                object child = container.GetChild(query.UriName);
                if (query.Subquery != null)
                {
                    return Find(child, query.Subquery);
                }
                else
                {
                    return child;
                }
            }
            return null;
        }

        public object Find(ObjectQuery query)
        {
            return Find(GlobalLists.GetInstance(), query);
        }

        public object Find(object searched, string query)
        {
            return Find(searched, ObjectQuery.parse(query));
        }

        public object Find(string query)
        {
            return Find(ObjectQuery.parse(query));
        }

        public IEnumerable FindAll(object searched, ObjectQuery query)
        {
            if (searched is IQueryable)
            {
                return GetListEnumerator(((IQueryable)searched).FindAll(query));
            }
            else
            {
                return null;
            }
        }

        private IEnumerable GetListEnumerator(IList<IQueryable> queryableList)
        {
            foreach (IQueryable obj in queryableList)
            {
                yield return obj;
            }
        }

        private IEnumerable SearchCollection(object coll, ObjectQuery query, int start, int length)
        {

            if (coll is IEnumerable)
                return SearchEnumerator(((IEnumerable)coll).GetEnumerator(), query, start, length);
            else if (coll is IEnumerator)
                return SearchEnumerator((IEnumerator) coll, query, start, length);
            else
                return null;
        }

        private IEnumerable SearchEnumerator(IEnumerator en, ObjectQuery query, int start, int length)
        {
            int i = 0;
            int matchCount = 0;
            QueryMatcher matcher = QueryMatcher.getMatcher(query);
            while (en.MoveNext() && (matchCount < length || length == 0))
            {
                if (i < start)
                {
                    continue;
                }
                bool isMatch = false;
                IUri uriObj = en.Current as IUri;
                if (query.MatchType == QueryMatchType.All) {
                    isMatch = true;
                } else {
                    if (uriObj != null)
                    {
                        if (query.MatchType == QueryMatchType.Exact)
                        {
                            isMatch = uriObj.Uri == query.UriName;
                        }
                        else
                        {
                            isMatch = uriObj.Uri.StartsWith(query.UriName);
                        }
                    }
                }
                if (isMatch && query.TypeName != null)
                {
                    if (query.TypeName != en.Current.GetType().Name
                        && query.TypeName != en.Current.GetType().FullName)
                    {
                        isMatch = false;
                    }
                }
                if (isMatch)
                {
                    matchCount++;
                    yield return en.Current;
                }
            }
            yield break;
        }

    }
}

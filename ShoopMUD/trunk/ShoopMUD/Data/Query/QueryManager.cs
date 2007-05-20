using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Collections.Specialized;
using System.Reflection;

namespace Shoop.Data.Query
{

    /// <summary>
    /// Refactor this class so that it can search objects that are not IQueryable
    /// </summary>
    public class QueryManager
    {
        private object[] emptyList;

        public QueryManager()
        {
            emptyList = new object[0];
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
        public object FindX(object searched, ObjectQuery query)
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

        public object Find(object searched, ObjectQuery query)
        {
            return Find(searched, query, 0);
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

        public object Find(object searched, ObjectQuery query, int index)
        {
            IEnumerable result = FindAll(searched, query, index, 1);
            foreach (object o in result)
            {
                return o;
            }
            return null;
        }
        public object Find(ObjectQuery query, int index)
        {
            return Find(GlobalLists.GetInstance(), query, index);
        }

        public object Find(object searched, string query, int index)
        {
            return Find(searched, ObjectQuery.parse(query), index);
        }

        public object Find(string query, int index)
        {
            return Find(GlobalLists.GetInstance(), ObjectQuery.parse(query), index);
        }


        public IEnumerable FindAll(object searched, ObjectQuery query, int start, int count)
        {
            return FindAll(searched, query, start, count, 0);
        }

          //only real thing to implement is FindAll
        private IEnumerable FindAll(object searched, ObjectQuery query, int start, int count, QueryCollectionFlags flags)
        {
            if (query.IsAbsolute)
            {
                searched = GlobalLists.GetInstance();
            }
            if (searched == null)
                return emptyList;

            bool hasSubQuery = query.Subquery != null;

            if (!hasSubQuery)
            {
                if (searched is IUriContainer)
                {
                    IUriContainer cont = (IUriContainer)searched;
                    object child = cont.GetChild(query.UriName);
                    if (IsCollection(child))
                    {
                        return SearchCollection(child, new ObjectQuery("*"), start, count, cont.GetChildHints(query.UriName));
                    }
                    else
                    {
                        ArrayList al = new ArrayList();
                        // if they aren't starting at first item, there are no matches then
                        if (start == 0)
                        {
                            al.Add(child);
                        }
                        return al;
                    }
                }
                else if (IsCollection(searched))
                {
                    if (CanUseIndexer(searched, flags, query)) {
                        object child = ((IDictionary)searched)[query.UriName];
                        ArrayList al = new ArrayList();
                        // if they aren't starting at first item, there are no matches then
                        if (start == 0)
                        {
                            al.Add(child);
                        }
                        return al;
                    }
                    else
                    {
                        return SearchCollection(searched, query, start, count, flags);
                    }
                }
            }
            else
            {
                if (searched is IUriContainer)
                {
                    IUriContainer cont = (IUriContainer)searched;
                    object child = cont.GetChild(query.UriName);
                    return FindAll(child, query.Subquery, start, count, cont.GetChildHints(query.UriName));
                }
                else if (IsCollection(searched))
                {
                    object child;
                    if (CanUseIndexer(searched, flags, query))
                    {
                        child = ((IDictionary)searched)[query.UriName];
                    }
                    else
                    {
                        child = FindFirstMatch(searched, query);
                    }
                    return FindAll(child, query.Subquery, start, count);
                }
            }
            return null;
        }

        private bool CanUseIndexer(object searched, QueryCollectionFlags flags, ObjectQuery query)
        {
            if (searched is IDictionary
                && (flags & QueryCollectionFlags.UriKeyedDictionary) == QueryCollectionFlags.UriKeyedDictionary
                && (query.MatchType == QueryMatchType.Exact
                    || (query.MatchType == QueryMatchType.Default)))
            {
                return true;
            }
            else
            {
                return false;
            }


        }

        public IEnumerable SearchCollection(object searched, ObjectQuery query, int start, int count, QueryCollectionFlags flags)
        {
            if (searched == null)
                yield break;

            QueryMatcher matcher = QueryMatcher.getMatcher(query);
            int matchCount = 0;
            int index = 0;
            foreach (IUri uriObj in GetCollectionEnumerable(searched))
            {
                if (count != 0 && matchCount == count) {
                    yield break;
                }

                // pick the first match
                if (matcher.IsMatch(uriObj))
                {
                    if (index++ >= start)
                    {
                        matchCount++;
                        yield return uriObj;
                    }
                }
            }
            yield break;
        }

        private object FindFirstMatch(object searched, ObjectQuery query)
        {
            foreach (object o in SearchCollection(searched, query, 0, 1, 0))
            {
                return o;
            }
            return null;
        }

        private bool IsCollection(object coll)
        {
            return (coll is IEnumerable);
        }

        private IEnumerable GetCollectionEnumerable(object coll)
        {
            IEnumerable e = null;
            if (coll is IDictionary)
            {
                e = ((IDictionary)coll).Values;
            }
            // if not dictionary just try regular enumerator
            if (e == null)
            {
                e = coll as IEnumerable;
            }
            return e;
        }
    }
}

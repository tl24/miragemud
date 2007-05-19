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
            IEnumerable result = FindAll(searched, query, 0, 1);
            foreach (object o in result)
            {
                return o;
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


          //only real thing to implement is FindAll
        public IEnumerable FindAll(object searched, ObjectQuery query, int start, int count)
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
                        al.Add(child);
                        return al;
                    }
                }
                else if (IsCollection(searched))
                {
                    return SearchCollection(searched, query, start, count, 0);
                }
            }
            else
            {
                if (searched is IUriContainer)
                {
                    IUriContainer cont = (IUriContainer)searched;
                    object child = cont.GetChild(query.UriName);
                    return FindAll(child, query.Subquery, start, count);
                }
                else if (IsCollection(searched))
                {
                    object child = FindFirstMatch(searched, query);
                    return SearchCollection(child, query.Subquery, start, count, 0);
                }
            }
            return null;
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
                    if (index >= start)
                    {
                        index++;
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
        private IEnumerable GetListEnumerator(IList<IQueryable> queryableList)
        {
            foreach (IQueryable obj in queryableList)
            {
                yield return obj;
            }
        }

        private IEnumerable GetCollectionEnumerable(object coll)
        {
            IEnumerable e = coll as IEnumerable;
            return e;
        }



    }
}

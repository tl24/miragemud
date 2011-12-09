using System;
using System.Collections;

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

    /// <summary>
    /// Refactor this class so that it can search objects that are not IQueryable
    /// </summary>
    public class QueryManager : IQueryManager
    {
        private object[] emptyList;
        private object _root;
        /// <summary>
        /// Creates a query manager using the default repository
        /// </summary>
        public QueryManager()
            : this(null)
        {
            
        }

        /// <summary>
        /// Create a query manager using the specified root to search for global uri's
        /// </summary>
        /// <param name="root">mud repository root</param>
        public QueryManager(object root)
        {
            emptyList = new object[0];
            this._root = root;
        }

        /// <summary>
        /// Finds the first object matching the given query
        /// </summary>
        /// <param name="searched">the object to search</param>
        /// <param name="query">the uri query</param>
        /// <returns>the first matching object</returns>
        public object Find(object searched, ObjectQuery query)
        {
            return Find(searched, query, 0);
        }

        /// <summary>
        /// Finds the first object matching the given query
        /// </summary>
        /// <param name="searched">the object to search</param>
        /// <param name="query">the uri query</param>
        /// <returns>the first matching object</returns>
        public object Find(object searched, string query)
        {
            return Find(searched, ObjectQuery.parse(query));
        }

        /// <summary>
        /// Searches the global lists for the first object matching the given query
        /// </summary>
        /// <param name="searched">the object to search</param>
        /// <param name="query">the uri query</param>
        /// <returns>the first matching object</returns>
        public object Find(ObjectQuery query)
        {
            return Find(Root, query);
        }

        /// <summary>
        /// Searches the global lists for the first object matching the given query
        /// </summary>
        /// <param name="searched">the object to search</param>
        /// <param name="query">the uri query</param>
        /// <returns>the first matching object</returns>
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
            return Find(Root, query, index);
        }

        public object Find(object searched, string query, int index)
        {
            return Find(searched, ObjectQuery.parse(query), index);
        }

        public object Find(string query, int index)
        {
            return Find(Root, ObjectQuery.parse(query), index);
        }

        /// <summary>
        /// Finds all matches for a given query.
        /// </summary>
        /// <param name="searched">object to be searched</param>
        /// <param name="query">the query</param>
        /// <param name="start">starting match</param>
        /// <param name="count">number of matches to return or 0 for all matches</param>
        /// <returns></returns>
        public IEnumerable FindAll(object searched, ObjectQuery query, int start, int count)
        {
            return FindAll(searched, query, start, count, 0);
        }

        /// <summary>
        /// This is the main search method that all other finds call.  Searches an object using the
        /// query given and returns all results as an enumerable object.
        /// </summary>
        /// <param name="searched">the object to search</param>
        /// <param name="query">the query</param>
        /// <param name="start">indicates the starting match to return</param>
        /// <param name="count">the number of matches to return or 0 for all</param>
        /// <param name="flags">any query hints available</param>
        /// <returns></returns>
        private IEnumerable FindAll(object searched, ObjectQuery query, int start, int count, QueryHints flags)
        {
            if (query.IsAbsolute)
            {
                searched = Root;
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

                    ArrayList al = new ArrayList();
                    // if they aren't starting at first item, there are no matches then
                    if (start == 0)
                    {
                        al.Add(child);
                    }
                    return al;

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

        /// <summary>
        /// Checks to see if a dictionary's indexer or get method can be used.  The object must be
        /// a dictionary keyed on the uri of the values, and an exact match query is being used.
        /// </summary>
        /// <param name="searched">object to search</param>
        /// <param name="flags">query hints</param>
        /// <param name="query">the query object</param>
        /// <returns></returns>
        private bool CanUseIndexer(object searched, QueryHints flags, ObjectQuery query)
        {
            if (searched is IDictionary
                && (flags & QueryHints.UriKeyedDictionary) == QueryHints.UriKeyedDictionary
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

        /// <summary>
        /// Helper method to search a collection
        /// </summary>
        /// <param name="searched"></param>
        /// <param name="query"></param>
        /// <param name="start"></param>
        /// <param name="count"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        public IEnumerable SearchCollection(object searched, ObjectQuery query, int start, int count, QueryHints flags)
        {
            if (searched == null)
                yield break;

            int matchCount = 0;
            int index = 0;
            if ((flags & QueryHints.DefaultPartialMatch) != 0)
                query.MatchType = QueryMatchType.Partial;

            foreach (IUri uriObj in GetCollectionEnumerable(searched))
            {
                if (count != 0 && matchCount == count) {
                    yield break;
                }

                // pick the first match
                if (query.IsMatch(uriObj))
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

        /// <summary>
        /// Gets the root object that will be searched when an absolute query is performed or if no searched object is set.
        /// If nothing is set, then the implementation for MudRepositoryBase will be used.
        /// </summary>
        public object Root
        {
            get {
                return _root;
            }
            set { _root = value; }
        }
        /// <summary>
        /// Finds the first match in a collection and returns it.
        /// </summary>
        /// <param name="searched"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        private object FindFirstMatch(object searched, ObjectQuery query)
        {
            foreach (object o in SearchCollection(searched, query, 0, 1, 0))
            {
                return o;
            }
            return null;
        }

        /// <summary>
        /// Checks to see if the object is a collection.
        /// </summary>
        /// <param name="coll">object to check</param>
        /// <returns></returns>
        private bool IsCollection(object coll)
        {
            return (coll is IEnumerable);
        }

        /// <summary>
        /// Gets an enumerable object for a collection.  The enumerable is
        /// then used in a foreach statement to search the collection
        /// </summary>
        /// <param name="coll"></param>
        /// <returns></returns>
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

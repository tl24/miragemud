using System;
using System.Collections;
using System.Text;
using System.Collections.Generic;


namespace Shoop.Data.Query
{
    [Flags]
    public enum QueryCollectionFlags
    {
        DuplicatesAllowed,
        Sorted,
        DefaultPartialMatch
    }

    /// <summary>
    /// This is an adapter for wrapping collections with IQueryable interface.
    /// The collection must only contain objects implementing IQueryable.  Other
    /// subclasses of this class may be used for specific collection types.
    /// </summary>
    public abstract class AbstractQueryableCollection : IQueryable, IEnumerable<IQueryable>
    {
        /// <summary>
        /// Attribute flags for the collection
        /// </summary>
        protected QueryCollectionFlags _flags;

        /// <summary>
        /// The uri of this collection
        /// </summary>
        protected string _uri;

        /// <summary>
        /// Creates an adapter for a collection.  The adapter will allow the objects inside to
        /// be queryable through the URINameable interface
        /// </summary>
        /// <param name="uri">the uri for this collection</param>
        /// <param name="collection">the collection to wrap</param>
        /// <param name="flags">Attributes about the collection</param>
        public AbstractQueryableCollection(string uri, QueryCollectionFlags flags)
        {
            this._flags = flags;
            this._uri = uri;
        }

        /// <summary>
        /// Creates an adapter for a collection.  The adapter will allow the objects inside to
        /// be queryable through the URINameable interface
        /// </summary>
        /// <param name="uri">the uri for this collection</param>
        /// <param name="collection">the collection to wrap</param>
        public AbstractQueryableCollection(string uri) : this(uri, 0) { }

        #region IQueryable Members

        /// <summary>
        /// Resource identifier for this object
        /// </summary>
        public string URI
        {
            get { return _uri; }
            set { _uri = value; }
        }

        /// <summary>
        /// Find the first object matching the given query
        /// </summary>
        /// <param name="query">the query</param>
        /// <returns>first matching object</returns>
        public virtual IQueryable Find(ObjectQuery query)
        {
            return Find(query, 1);
        }

        public virtual IList<IQueryable> FindAll(ObjectQuery query)
        {
            List<IQueryable> list = new List<IQueryable>();
            QueryMatcher matcher = QueryMatcher.getMatcher(query);
            
            foreach (IQueryable uriObj in this)
            {
                // pick the first match
                if (matcher.IsMatch(uriObj))
                {
                    if (query.Subquery != null)
                    {
                        return uriObj.FindAll(query.Subquery);
                    }
                    else
                    {
                        list.Add(uriObj);
                    }
                }
                else
                {
                    if ((_flags & QueryCollectionFlags.Sorted) != 0)
                    {
                        if (uriObj.URI.CompareTo(query.UriName) > 0)
                        {
                            return null;
                        }
                    }
                }
            }
            return list;
        }

        public virtual IQueryable Find(ObjectQuery query, int index)
        {
            int match = 0;
            QueryMatcher matcher = QueryMatcher.getMatcher(query);
            foreach (IQueryable uriObj in this)
            {
                // pick the first match
                if (matcher.IsMatch(uriObj))
                {
                    if (query.Subquery != null)
                    {
                        return uriObj.Find(query.Subquery, index);
                    }
                    else
                    {
                        if (++match == index)
                            return uriObj;
                    }
                }
                else
                {
                    if ((_flags & QueryCollectionFlags.Sorted) == QueryCollectionFlags.Sorted)
                    {
                        if (uriObj.URI.CompareTo(query.UriName) > 0)
                        {
                            return null;
                        }
                    }
                }
            }
            return null;
        }

        public virtual IQueryable Find(string query)
        {
            return Find(ObjectQuery.parse(query));
        }

        public virtual IList<IQueryable> FindAll(string query)
        {
            return FindAll(ObjectQuery.parse(query));
        }

        public virtual IQueryable Find(string query, int index)
        {
            return Find(ObjectQuery.parse(query), index);
        }

        #endregion

        #region IEnumerable<IQueryable> Members

        public abstract IEnumerator<IQueryable> GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        protected WrappedEnumerator<T> GetWrappedEnumerator<T>(IEnumerator enumerator) where T : IQueryable
        {
            return new WrappedEnumerator<T>(enumerator);
        }

        public class WrappedEnumerator<T> : IEnumerator<IQueryable> where T : IQueryable
        {
            protected IEnumerator _enumerator;

            public WrappedEnumerator(IEnumerator _enumerator)
            {
                this._enumerator = _enumerator;
            }

            #region IEnumerator<T> Members

            public virtual IQueryable Current
            {
                get
                {
                    return GetCurrent();
                }

            }

            #endregion


            #region IDisposable Members

            public virtual void Dispose()
            {
                IDisposable dis = _enumerator as IDisposable;
                if (dis != null)
                {
                    dis.Dispose();
                }
                _enumerator = null;
            }

            #endregion

            protected virtual T GetCurrent()
            {
                return (T)_enumerator.Current;
            }

            #region IEnumerator Members

            object IEnumerator.Current
            {
                get { return GetCurrent(); }
            }

            public virtual bool MoveNext()
            {
                return _enumerator.MoveNext();
            }

            public virtual void Reset()
            {
                _enumerator.Reset();
            }

            #endregion
        }

    }
}

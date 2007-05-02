using System;
using System.Collections.Generic;
using System.Text;
using Shoop.Data.Query;

namespace Shoop.Data
{
    public class BaseData : IQueryable
    {
        protected string _uri;
        protected Dictionary<string, IQueryable> _uriProperties;

        protected BaseData()
        {
            _uri = this.ToString();
            _uriProperties = new Dictionary<string, IQueryable>();
        }

        #region IQueryable Members

        public string URI
        {
            get { return _uri; }
            set { _uri = value ?? value.ToLower(); }
        }

        public virtual IQueryable Find(ObjectQuery query)
        {
            validateQuery(query);
            IQueryable child = _uriProperties[query.UriName];
            if (query.Subquery != null)
            {
                return child.Find(query.Subquery);
            }
            else
            {
                return child;
            }
        }

        protected virtual void validateQuery(ObjectQuery query)
        {
            if (!_uriProperties.ContainsKey(query.UriName))
            {
                throw new ArgumentException(query.UriName + " does not exist or is not registered in " + this.GetType().ToString() + "." + this._uri);
            }
        }

        public virtual IList<IQueryable> FindAll(ObjectQuery query)
        {
            validateQuery(query);
            IQueryable child = _uriProperties[query.UriName];
            if (query.Subquery != null)
            {
                return child.FindAll(query.Subquery);
            }
            else
            {
                if (child is IEnumerable<IQueryable>)
                {
                    return new List<IQueryable>((IEnumerable<IQueryable>)child);
                }
                else
                {
                    IList<IQueryable> list = new List<IQueryable>();
                    list.Add(child);
                    return list;
                }
            }
        }

        public virtual IQueryable Find(ObjectQuery query, int index)
        {
            validateQuery(query);
            IQueryable child = _uriProperties[query.UriName];
            if (query.Subquery != null)
            {
                return child.Find(query.Subquery, index);
            }
            else
            {
                if (child is System.Collections.IEnumerable)
                {
                    int i = 0;
                    foreach (IQueryable uriObj in (System.Collections.IEnumerable) child)
                    {
                        if (i++ == index)
                            return uriObj;
                    }
                    return null;
                }
                else
                {
                    return (index == 1) ? child : null;
                }
            }
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
    }
}

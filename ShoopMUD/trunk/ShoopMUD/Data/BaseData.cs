using System;
using System.Collections.Generic;
using System.Text;
using Shoop.Data.Query;

namespace Shoop.Data
{
    public class BaseData : IUriContainer, IUri
    {
        protected string _uri;
        protected Dictionary<string, ChildCollectionPair> _uriChildCollections;

        protected BaseData()
        {
            _uri = this.ToString();
            _uriChildCollections = new Dictionary<string, ChildCollectionPair>();
        }

        #region IQueryable Members

        public string Uri
        {
            get { return _uri; }
            set { _uri = value ?? value.ToLower(); }
        }

        public virtual string FullUri
        {
            get { return this.Uri; }
        }

        public override string ToString()
        {
            return base.ToString() + " mud://" + FullUri;
        }
        #endregion

        #region IUriContainer Members

        public object GetChild(string uri)
        {
            return _uriChildCollections.ContainsKey(uri) ? _uriChildCollections[uri].Child : null;
        }

        public QueryCollectionFlags GetChildHints(string uri)
        {
            return _uriChildCollections.ContainsKey(uri) ? _uriChildCollections[uri].Flags : 0;
        }

        #endregion

        protected struct ChildCollectionPair
        {
            public object Child;
            public QueryCollectionFlags Flags;

            public ChildCollectionPair(object Child, QueryCollectionFlags flags) {
                this.Child = Child;
                this.Flags = flags;
            }

            public ChildCollectionPair(object Child) : this(Child, 0)
            {
            }

        }
    }
}

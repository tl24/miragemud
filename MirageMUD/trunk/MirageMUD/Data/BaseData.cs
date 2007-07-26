using System;
using System.Collections.Generic;
using System.Text;
using Mirage.Data.Query;

namespace Mirage.Data
{
    public class BaseData : IUriContainer, IUri
    {
        protected string _uri;
        protected Dictionary<string, ChildCollectionPair> _uriChildCollections;

        protected BaseData()
        {
            _uri = "";
            _uriChildCollections = new Dictionary<string, ChildCollectionPair>();
        }

        #region IQueryable Members

        [Editor(IsKey=true, Priority=1)]
        public string Uri
        {
            get { return _uri; }
            set { _uri = value ?? value.ToLower(); }
        }

        [Editor(Priority=2, IsReadonly=true)]
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

        public QueryHints GetChildHints(string uri)
        {
            return _uriChildCollections.ContainsKey(uri) ? _uriChildCollections[uri].Flags : 0;
        }

        #endregion

        /// <summary>
        /// struct to hold data about a child collection exposed under Uri interfaces
        /// </summary>
        protected struct ChildCollectionPair
        {
            public object Child;
            public QueryHints Flags;

            public ChildCollectionPair(object Child, QueryHints flags) {
                this.Child = Child;
                this.Flags = flags;
            }

            public ChildCollectionPair(object Child) : this(Child, 0)
            {
            }

        }
    }
}

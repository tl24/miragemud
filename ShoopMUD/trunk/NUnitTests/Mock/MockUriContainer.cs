using System;
using System.Collections.Generic;
using System.Text;
using Shoop.Data.Query;

namespace NUnitTests.Mock
{
    public class MockUriContainer : IUri, IUriContainer
    {
        private string _uri = "global";
        private IDictionary<string, object> _objects;
        private IDictionary<string, QueryHints> _flags;


        public MockUriContainer()
        {
            _objects = new Dictionary<string, object>();
            _flags = new Dictionary<string, QueryHints>();
        }

        #region IUri Members

        public string Uri
        {
            get { return _uri; }
            set { _uri = value; }
        }

        public string FullUri
        {
            get { return _uri; }
        }

        #endregion

        #region IUriContainer Members

        public object GetChild(string uri)
        {
            uri = uri.ToLower();
            return _objects.ContainsKey(uri) ? _objects[uri] : null;
        }

        public QueryHints GetChildHints(string uri)
        {
            uri = uri.ToLower();
            return _flags.ContainsKey(uri) ? _flags[uri] : 0;
        }

        public void AddObject(string uri, object o, QueryHints flags)
        {
            uri = uri.ToLower();
            _objects[uri] = o;
            _flags[uri] = flags;
        }

        #endregion
    }
}

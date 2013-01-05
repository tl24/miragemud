using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Mirage.Game.World.Query
{
    public class ObjectUri : IEnumerable<string>
    {
        private readonly string _uri;
        private static Regex _formatPattern = new Regex("^/?([^/]+/)*[^/]*$");
        public ObjectUri(string uri)
        {
            if (string.IsNullOrWhiteSpace(uri))
                throw new ArgumentException("uri is required", "uri");
            if (!_formatPattern.IsMatch(uri))
                throw new FormatException("invalid uri format"); 
            uri = uri.Trim();
            if (uri != "/")
                uri = uri.TrimEnd('/');
            _uri = uri;
        }

        public ObjectUri Append(string uri)
        {
            if (string.IsNullOrWhiteSpace(uri))
                throw new ArgumentException("uri is required", "uri");
            if (!_formatPattern.IsMatch(uri))
                throw new FormatException("invalid uri format");
            if (uri == "/")
                throw new ArgumentException("Cannot append root uri", "uri");

            uri = uri.Trim();
            if (_uri == "/")
                return new ObjectUri(_uri + uri.TrimStart('/'));
            else
                return new ObjectUri(_uri.TrimEnd('/') + "/" + uri.TrimStart('/'));
        }

        public string Uri
        {
            get
            {
                return _uri;
            }
        }

        public bool IsAbsolute
        {
            get { return _uri[0] == '/'; }
        }

        public override string ToString()
        {
            return _uri;
        }

        public override bool Equals(object obj)
        {
            if (base.Equals(obj))
                return true;
            if (!(obj is ObjectUri))
                return false;
            return _uri.Equals(((ObjectUri)obj)._uri);
        }
        public override int GetHashCode()
        {
            return _uri.GetHashCode();
        }

        #region IEnumerable<string> Members

        public IEnumerator<string> GetEnumerator()
        {
            var parts = _uri.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
            if (IsAbsolute)
                return Enumerable.Repeat("/", 1).Concat(parts).GetEnumerator();
            else
                return ((IEnumerable<string>) parts).GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion
    }
}

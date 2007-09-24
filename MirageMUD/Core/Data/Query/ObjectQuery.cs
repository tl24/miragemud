using System;
using System.Collections.Generic;
using System.Text;

namespace Mirage.Core.Data.Query
{

    public enum QueryMatchType
    {
        Default = 0,
        Exact,
        Partial,
        All
    }

    /// <summary>
    /// Specifies a query over a URINameable set of resources
    /// </summary>
    public class ObjectQuery
    {
        private string _typeName;
        private string _uriName;
        private ObjectQuery _subquery;
        private QueryMatchType _matchType;
        private string _toString;
        private bool _isAbsolute;

        public ObjectQuery(string type, string uri, ObjectQuery subQuery)
        {
            //TODO: escape special characters in the type and name ":/;"            
            this._typeName = type;
            this._uriName = uri;
            this._subquery = subQuery;

            if (this._uriName != null && this._uriName.StartsWith("/"))
            {
                this._uriName = this._uriName.Substring(1);
            }

            // parse the flag values
            if (this._uriName == "*")
            {
                _matchType = QueryMatchType.All;
            } else if (this._uriName.EndsWith("*")) {
                _matchType = QueryMatchType.Partial;
                this._uriName = this._uriName.Substring(0, this._uriName.Length - 1);
            }
        }

        public ObjectQuery(string uri) : this(null, uri, null) { }

        public ObjectQuery(string type, string uri) : this(type, uri, null) { }

        /// <summary>
        /// Parses a string representation of a uri query into a Uri query object
        /// </summary>
        /// <param name="uriQueryString"></param>
        /// <returns></returns>
        public static ObjectQuery parse(string uriQueryString, string subQueryUri)
        {
            bool isAbsolute = false;
            if (uriQueryString.StartsWith("/"))
            {
                uriQueryString = uriQueryString.Substring(1);
                isAbsolute = true;
            }
            
            string[] root = uriQueryString.Split(new char[] { '/' }, 2);
            ObjectQuery result = null;
            if (root[0] != string.Empty)
            {
                string[] pieces = root[0].Split(':');
                if (pieces.Length > 1)
                {
                    result = new ObjectQuery(pieces[0], pieces[1]);
                    result._isAbsolute = isAbsolute;
                }
                else if (pieces[0] != null && pieces[0] != string.Empty)
                {
                    result = new ObjectQuery(pieces[0]);
                    result._isAbsolute = isAbsolute;
                }
            }

            if (result != null && root.Length > 1)
            {
                result._subquery = parse(root[1], subQueryUri);
            }
            else if (result != null && subQueryUri != null)
            {
                result._subquery = new ObjectQuery(subQueryUri);
            }
            return result;
        }

        /// <summary>
        /// Parses the uriQueryString and sets the subquery of the result to the subQueryUri.  The subQueryUri
        /// is not parsed.  Use this parser for user entered input so that the query cannot be altered unexpectedly.
        /// </summary>
        /// <example>
        ///     <code>ObjectQuery query = ObjectQuery.parse("/Players", someUserInput);</code>
        /// </example>
        /// <param name="uriQueryString">the uri query string</param>
        /// <param name="subQueryUri">the subquery, will be the final subquery</param>
        /// <returns></returns>
        public static ObjectQuery parse(string uriQueryString) {
            return parse(uriQueryString, null);
        }
        public override string ToString()
        {
            if (_toString == null)
            {
                StringBuilder sb = new StringBuilder();
                if (_typeName != null && _typeName != string.Empty)
                {
                    sb.Append(_typeName);
                    sb.Append(':');
                }
                if (_uriName != null)
                {
                    sb.Append(_uriName);
                }

                if (_matchType == QueryMatchType.Partial || _matchType == QueryMatchType.All)
                {
                    sb.Append('*');
                }

                if (_subquery != null)
                {
                    sb.Append('/');
                    sb.Append(_subquery.ToString());
                }
                _toString = sb.ToString();
            }
            return _toString;
        }

        public static bool operator ==(ObjectQuery q1, ObjectQuery q2) {
            if (object.ReferenceEquals(q1, q2))
                return true;

            if ((object)q1 == null)
                return (object)q2 == null;
            else
                return q1.Equals(q2);
        }
      
        public static bool operator !=(ObjectQuery q1, ObjectQuery q2)
        {
            return !(q1 == q2);
        }

        public override bool Equals(object obj)
        {
            if (!base.Equals(obj))
            {
                ObjectQuery other = obj as ObjectQuery;
                if (other != null)
                {
                    return this.ToString().Equals(other.ToString());
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return true;
            }
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        public string TypeName
        {
            get { return this._typeName; }
            set { this._typeName = value; }
        }

        public string UriName
        {
            get { return this._uriName; }
            set { this._uriName = value; }
        }

        public ObjectQuery Subquery
        {
            get { return this._subquery; }
            set { this._subquery = value; }
        }

        public QueryMatchType MatchType
        {
            get { return _matchType;  }
            set { _matchType = value; }
        }

        public bool IsAbsolute
        {
            get { return this._isAbsolute; }
        }

        /// <summary>
        /// Checks the object against the given query to see if there is a match
        /// </summary>
        /// <param name="obj">the object to match</param>
        /// <returns>true if the object matches</returns>
        public bool IsMatch(IUri obj)
        {
            QueryMatchType matchType = GetMatchType(QueryMatchType.Default);
            // simple match for now
            if (matchType == QueryMatchType.Exact)
            {
                if (!(obj.Uri.Equals(this.UriName, StringComparison.CurrentCultureIgnoreCase)))
                    return false;
            }
            else if (matchType == QueryMatchType.Partial)
            {
                if (!obj.Uri.StartsWith(this.UriName, StringComparison.CurrentCultureIgnoreCase))
                    return false;
            }
            if (this.TypeName != null)
            {
                if (obj.GetType().Name != this.TypeName
                    && obj.GetType().FullName != this.TypeName)
                {
                    return false;
                }
            }
            return true;
        }

        private QueryMatchType GetMatchType(QueryMatchType desired)
        {
            QueryMatchType result = desired;
            if (result == QueryMatchType.Default)
            {
                result = this.MatchType;
            }
            if (result == QueryMatchType.Default)
            {
                result = QueryMatchType.Exact;
            }
            return result;
        }
    }
}

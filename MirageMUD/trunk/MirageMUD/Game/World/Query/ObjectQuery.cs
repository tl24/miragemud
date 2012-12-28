using System;
using System.Text;

namespace Mirage.Game.World.Query
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

        public ObjectQuery(string type, string uri, ObjectQuery subQuery)
        {
            //TODO: escape special characters in the type and name ":/;"            
            this.TypeName = type;
            this.UriName = uri;
            this.Subquery = subQuery;

            if (this.UriName != null && this.UriName.StartsWith("/"))
            {
                this.UriName = this.UriName.Substring(1);
            }

            // parse the flag values
            if (this.UriName == "*")
            {
                MatchType = QueryMatchType.All;
            } else if (this.UriName.EndsWith("*")) {
                MatchType = QueryMatchType.Partial;
                this.UriName = this.UriName.Substring(0, this.UriName.Length - 1);
            }
        }

        public ObjectQuery(string uri) : this(null, uri, null) { }

        public ObjectQuery(string type, string uri) : this(type, uri, null) { }

        /// <summary>
        /// Parses a string representation of a uri query into a Uri query object
        /// </summary>
        /// <param name="uriQueryString"></param>
        /// <returns></returns>
        public static ObjectQuery Parse(string uriQueryString, string subQueryUri)
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
                    result.IsAbsolute = isAbsolute;
                }
                else if (pieces[0] != null && pieces[0] != string.Empty)
                {
                    result = new ObjectQuery(pieces[0]);
                    result.IsAbsolute = isAbsolute;
                }
            }

            if (result != null && root.Length > 1)
            {
                result.Subquery = Parse(root[1], subQueryUri);
            }
            else if (result != null && subQueryUri != null)
            {
                result.Subquery = new ObjectQuery(subQueryUri);
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
        public static ObjectQuery Parse(string uriQueryString) {
            return Parse(uriQueryString, null);
        }

        private string _toString;
        public override string ToString()
        {
            if (_toString == null)
            {
                StringBuilder sb = new StringBuilder();
                if (TypeName != null && TypeName != string.Empty)
                {
                    sb.Append(TypeName);
                    sb.Append(':');
                }
                if (UriName != null)
                {
                    sb.Append(UriName);
                }

                if (MatchType == QueryMatchType.Partial || MatchType == QueryMatchType.All)
                {
                    sb.Append('*');
                }

                if (Subquery != null)
                {
                    sb.Append('/');
                    sb.Append(Subquery.ToString());
                }
                _toString = sb.ToString();
            }
            return _toString;
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

        public string TypeName { get; set; }

        public string UriName { get; set; }

        public ObjectQuery Subquery { get; set; }

        public QueryMatchType MatchType { get; set; } 

        public bool IsAbsolute { get; private set; }

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

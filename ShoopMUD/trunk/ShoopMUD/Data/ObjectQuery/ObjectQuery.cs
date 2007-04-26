using System;
using System.Collections.Generic;
using System.Text;

namespace Shoop.Data.Query
{
    /// <summary>
    /// Specifies a query over a URINameable set of resources
    /// </summary>
    public class ObjectQuery
    {
        private string _typeName;
        private string _uriName;
        private ObjectQuery _subquery;

        public ObjectQuery(string type, string uri, ObjectQuery subQuery)
        {
            //TODO: escape special characters in the type and name ":/;"            
            this._typeName = type;
            this._uriName = uri;
            this._subquery = subQuery;
        }

        public ObjectQuery(string uri) : this(null, uri, null) { }

        public ObjectQuery(string type, string uri) : this(type, uri, null) { }

        /// <summary>
        /// Parses a string representation of a uri query into a URI query object
        /// </summary>
        /// <param name="uriQueryString"></param>
        /// <returns></returns>
        public static ObjectQuery parse(string uriQueryString)
        {
            string[] root = uriQueryString.Split(new char[] { '/' }, 2);
            ObjectQuery result = null;
            if (root[0] != string.Empty)
            {
                string[] pieces = root[0].Split(':');
                if (pieces.Length > 1)
                {
                    result = new ObjectQuery(pieces[0], pieces[1]);
                }
            }

            if (result != null && root.Length > 1)
            {
                result.Subquery = parse(root[1]);
            }
            return result;
        }

        public override string ToString()
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
            if (_subquery != null)
            {
                sb.Append('/');
                sb.Append(_subquery.ToString());
            }
            return sb.ToString();
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
    }
}

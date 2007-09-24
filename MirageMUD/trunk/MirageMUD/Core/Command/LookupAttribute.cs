using System;
using System.Collections.Generic;
using System.Text;
using Mirage.Core.Data.Query;

namespace Mirage.Core.Command
{
    [AttributeUsageAttribute(System.AttributeTargets.Parameter)]
    public class LookupAttribute : BaseArgumentAttribute
    {
        private string _baseUri;
        private QueryMatchType _matchType = QueryMatchType.Default;
        private bool _isRequired = true;
        private string _errorResource = "Error.NotHere";

        /// <summary>
        /// Looks up an object in the world with an ObjectQuery.
        /// </summary>
        /// <param name="baseUri">The base uri of the object query, the player argument will be appended to the base uri</param>
        public LookupAttribute(string baseUri)
        {
            _baseUri = baseUri;
        }

        public string BaseUri
        {
            get { return this._baseUri; }
            set { this._baseUri = value; }
        }

        public QueryMatchType MatchType
        {
            get { return this._matchType; }
            set { this._matchType = value; }
        }

        public bool IsRequired
        {
            get { return this._isRequired; }
            set { this._isRequired = value; }
        }

        public string ErrorResource
        {
            get { return this._errorResource; }
            set { this._errorResource = value; }
        }

        public ObjectQuery ConstructQuery(string argument)
        {
            ObjectQuery query = ObjectQuery.parse(_baseUri, argument);
            query.MatchType = _matchType;
            return query;
        }
    }
}

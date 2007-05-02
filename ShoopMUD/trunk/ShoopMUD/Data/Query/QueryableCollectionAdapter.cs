using System;
using System.Collections.Generic;
using System.Text;
using Shoop.Data.Query;

namespace Shoop.Data.Query
{
    public class QueryableCollectionAdapter<T> : AbstractQueryableCollection where T : IQueryable
    {

        /// <summary>
        /// The wrapped collection of this instance
        /// </summary>
        protected ICollection<T> col;

        public QueryableCollectionAdapter(ICollection<T> collection, string uri) : this(collection, uri, 0) { }

        public QueryableCollectionAdapter(ICollection<T> collection, string uri, QueryCollectionFlags _flags)
            : base(uri, _flags)
        {
            this.col = collection;
        }



        public override IEnumerator<IQueryable> GetEnumerator()
        {
            return (IEnumerator<IQueryable>) GetWrappedEnumerator<T>(col.GetEnumerator());
        }
    }
}

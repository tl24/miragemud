using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace Shoop.Data.Query
{
    public class QueryableDictionaryAdapter<V> : AbstractQueryableCollection 
        where V : IQueryable
    {
        private IDictionary<string, V> sortCol;

        public QueryableDictionaryAdapter(string uri, IDictionary<string, V> collection)
            :
            this(uri, collection, 0)
        {
            
        }

        public QueryableDictionaryAdapter(string uri, IDictionary<string, V> collection, QueryCollectionFlags flags)
            : base(uri, flags | QueryCollectionFlags.Sorted)
        {
            this.sortCol = collection;
        }

        public override IQueryable find(ObjectQuery query)
        {

            IQueryable child = sortCol[query.UriName];
            if (query.Subquery != null)
            {
                return child.find(query.Subquery);
            }
            else
            {
                return child;
            }
        }

        public override IEnumerator<IQueryable> GetEnumerator()
        {
            return (IEnumerator<IQueryable>) new WrappedDictionaryEnumerator(sortCol, ((IEnumerable)sortCol).GetEnumerator());
        }

        public class WrappedDictionaryEnumerator : AbstractQueryableCollection.WrappedEnumerator<V> , IEnumerator<V>
        {
            protected IDictionary<string, V> sortCol;

            internal WrappedDictionaryEnumerator(IDictionary<string, V> sortCol, IEnumerator _enumerator)
                : base(_enumerator)
            {
                this.sortCol = sortCol;
            }

            protected override V GetCurrent()
            {
                string key = (string)_enumerator.Current;
                return sortCol[key];
            }
        }

    }
}

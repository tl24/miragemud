using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace Shoop.Data.Query
{
    public class QuerySortedDictionaryAdapter<V> : AbstractQueryableCollection 
        where V : IQueryable
    {
        private SortedDictionary<string, V> sortCol;

        public QuerySortedDictionaryAdapter(string uri, SortedDictionary<string, V> collection)
            :
            this(uri, collection, 0)
        {
            
        }

        public QuerySortedDictionaryAdapter(string uri, SortedDictionary<string, V> collection, QueryCollectionFlags flags)
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
            return new SortedWrappedEnumerator(((IEnumerable)sortCol).GetEnumerator());
        }

        protected class SortedWrappedEnumerator : AbstractQueryableCollection.WrappedEnumerator<V>
        {
            protected SortedDictionary<string, V> sortCol;

            protected SortedWrappedEnumerator(SortedDictionary<string, V> sortCol, IEnumerator _enumerator) : base(_enumerator) {
                this.sortCol = sortCol;
            }

            protected override V GetCurrent()
            {
                string key = _enumerator.Current as string;
                return sortCol[key];
            }
        }

    }
}

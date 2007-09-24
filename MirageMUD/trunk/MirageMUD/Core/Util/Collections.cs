using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace Mirage.Core.Util
{
    /// <summary>
    /// Default implementation of the IIndexDictionary interface.  It provides a keyed-collection
    /// that is indexed for fast lookup of strings.  The collection does allow duplicates, and the
    /// elements are returned in sorted order.
    /// </summary>
    /// <typeparam name="TVal">The type of object the collection holds</typeparam>
    public class IndexedDictionary<TVal> : IIndexedDictionary<TVal>
    {
        private LinkedList<IndexPage> index;
        private LinkedList<KeyNode> list;
        private int _count;
        private const int DEFAULT_MAX_INDEX_SIZE = 4;

        private int maxIndexSize = DEFAULT_MAX_INDEX_SIZE;
        private enum ComparisonType {
            Insertion,
            Partial,
            Exact
        }

        public IndexedDictionary()
        {
            index = new LinkedList<IndexPage>();
            list = new LinkedList<KeyNode>();
            _count = 0;
        }

        #region IIndexedDictionary<TVal> Members

        public IList<TVal> GetAllValues()
        {
            return new List<TVal>(this);
        }


        public IList<TVal> FindStartsWith(string substring)
        {
            return find(substring, ComparisonType.Partial);
        }

        public IList<TVal> FindExact(string key)
        {
            return find(key, ComparisonType.Exact);
        }

        private IList<TVal> find(string key, ComparisonType compType)
        {
            Debug.Assert(compType != ComparisonType.Insertion, "Insertion mode not valid for Find");
            List<TVal> items = new List<TVal>();
            key = normalizeKey(key);
            LinkedListNode<IndexPage> page = findPage(key, ComparisonType.Partial);
            if (page != null)
            {
                LinkedListNode<KeyNode> item = null;
                for (item = page.Value.start; item != null; item = item.Next)
                {
                    if (compType == ComparisonType.Partial && item.Value.Key.StartsWith(key)
                        || compType == ComparisonType.Exact && item.Value.Key.Equals(key))
                    {
                        break;
                    }
                }
                for (; item != null; item = item.Next)
                {
                    if (compType == ComparisonType.Partial && item.Value.Key.StartsWith(key)
                        || compType == ComparisonType.Exact && item.Value.Key.Equals(key))
                    {
                        items.Add(item.Value.value);
                    }
                    else
                    {
                        break;
                    }
                }
            }
            return items;
        }

        public void Put(string key, TVal item)
        {
            if (key == null || key.Length == 0)
            {
                throw new ArgumentException("Key can not be null or empty");
            }
            key = normalizeKey(key);
            LinkedListNode<IndexPage> pageNode = findPage(key, ComparisonType.Insertion);
            Debug.Assert(pageNode != null, "Index page should always be found for insertion");
            pageNode.Value.add(new KeyNode(key, item));
            if (pageNode.Value.size > maxIndexSize)
            {
                index.AddAfter(pageNode, pageNode.Value.splitPage());
            }
            _count++;
        }

        private string normalizeKey(string key)
        {
            return (key == null ? null : key.ToLower());
        }

        /// <summary>
        ///     Searches the index to Find the first page that the key could
        /// occur on or fit if its an insertion
        /// </summary>
        /// <param name="key">the key to search on</param>
        /// <param name="type">The type of operation to be performed</param>
        /// <returns>the node for the index page</returns>
        private LinkedListNode<IndexPage> findPage(string key, ComparisonType type)
        {
            if (index.Count == 0)
            {
                if (type == ComparisonType.Insertion)
                {
                    return index.AddFirst(new IndexPage(list));
                }
                else
                {
                    return null;
                }
            }

            for (LinkedListNode<IndexPage> page = index.First; page != null; page = page.Next) {
                int comp = page.Value.compare(key);
                if (comp == 0)
                {
                    return page;
                }
                else if (comp < 0)
                {
                    if (type != ComparisonType.Exact)
                    {
                        // if there's a previous page, then that is the one
                        if (page.Previous != null)
                        {
                            return (page.Previous.Value.size < page.Value.size ? page.Previous : page);
                        }
                        else
                        {
                            return page;
                        }
                    }
                }
            }

            if (type == ComparisonType.Exact)
            {
                // we reached the end without finding it
                return null;
            }
            else
            {
                return index.Last;
            }
        }

        public int Count
        {
            get { return _count; }
        }

        #endregion

        #region IEnumerable<TVal> Members

        public IEnumerator<TVal> GetEnumerator()
        {
            return new IndexedDictionaryEnumerator(this, this.list.First, EnumerationMode.All, null);
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        #endregion

        private enum EnumerationMode
        {
            All,
            Partial,
            Exact
        }

        private class IndexedDictionaryEnumerator : IEnumerator<TVal>
        {
            private IndexedDictionary<TVal> enumerated;
            private LinkedListNode<KeyNode> lastNode;
            private LinkedListNode<KeyNode> start;
            private EnumerationMode mode;
            private string key;
            private bool done;
            public IndexedDictionaryEnumerator(IndexedDictionary<TVal> enumerated, LinkedListNode<KeyNode> start, EnumerationMode mode, string key)
            {
                this.enumerated = enumerated;
                this.start = start;
                this.mode = mode;
                this.key = key;
                this.done = false;
            }

            #region IEnumerator<TVal> Members

            public TVal Current
            {
                get {
                    if (lastNode == null)
                    {
                        throw new InvalidOperationException("MoveNext must be called first");
                    }
                    return lastNode.Value.value;
                }
            }

            #endregion

            #region IDisposable Members

            public void Dispose()
            {
                enumerated = null;
                start = null;
                lastNode = null;
                done = true;
            }

            #endregion

            #region IEnumerator Members

            object System.Collections.IEnumerator.Current
            {
                get { return this.Current; }
            }

            public bool MoveNext()
            {
                if (done)
                    return false;

                if (lastNode != null)
                {
                    lastNode = lastNode.Next;
                    if (mode == EnumerationMode.Partial)
                    {
                        if (lastNode != null && !lastNode.Value.Key.StartsWith(key))
                        {
                            lastNode = null;
                        }
                    }
                    else if (mode == EnumerationMode.Exact)
                    {
                        if (lastNode != null && !lastNode.Value.Key.Equals(key))
                        {
                            lastNode = null;
                        }
                    }
                }
                else
                {
                    lastNode = start;
                }
                return lastNode != null;
            }

            public void Reset()
            {
                lastNode = null;
                done = false;
            }

            #endregion
        }

        private class IndexPage {
            public LinkedListNode<KeyNode> start;
            public LinkedListNode<KeyNode> end;
            public LinkedList<KeyNode> list;
            public int size;

            public IndexPage(LinkedList<KeyNode> list)
            {
                this.list = list;
                size = 0;
                start = end = null;
            }
            /// <summary>
            /// Add the key node to this index page.  It is assumed that
            /// a check has already been made to determine that this is the correct
            /// page for the node.
            /// </summary>
            /// <param name="node">the node to add</param>
            public void add(KeyNode node) {
                LinkedListNode<KeyNode> added = null;

                // search through the list to Find the insertion point
                for (LinkedListNode<KeyNode> pNode = start; pNode != end && pNode != null; pNode = pNode.Next)
                {
                    if (node.Key.CompareTo(pNode.Value.Key) < 0)
                    {
                        added = pNode.List.AddBefore(pNode, node);
                        if (pNode == start)
                        {
                            start = added;
                        }
                        break;
                    }
                }
                // node wasn't added
                if (added == null)
                {
                    if (start == null || end == null) {
                        start = end = added = list.AddFirst(node);
                    } else {
                        // check to see if it comes before or after the last node
                        if (node.Key.CompareTo(end.Value.Key) < 0)
                        {
                            added = list.AddBefore(end, node);
                            if (start == end)
                            {
                                start = added;
                            }
                        }
                        else
                        {
                            added = list.AddAfter(end, node);
                            end = added;
                        }
                    }
                }
                size++;
            }

            /// <summary>
            /// Splits the current page in half creating 2 pages.
            /// The new page will come sorted after the existing page.
            /// </summary>
            /// <returns>The new page</returns>
            public IndexPage splitPage()
            {
                int splitIndex = size / 2;
                if (splitIndex >= 1)
                {
                    LinkedListNode<KeyNode> node = start;
                    for (int i = 0; i < splitIndex; i++)
                    {
                        node = node.Next;
                    }
                    IndexPage newPage = new IndexPage(this.list);
                    newPage.end = this.end;
                    newPage.start = node;
                    this.end = node.Previous;
                    newPage.size = size - splitIndex;
                    this.size -= newPage.size;
                    return newPage;
                }
                else
                {
                    return null;
                }
            }

            /// <summary>
            /// Compares the given key with this index page.  If the
            /// key comes before the beginning of the page, -1 is returned.
            /// If the key would be contained in the index page, 0 is returned.
            /// If the key comes after the end of the page, 1 is returned.
            /// </summary>
            /// <param name="key">The key to compare</param>
            /// <returns>comparison result</returns>
            public int compare(string key) {
                int comp = this.start.Value.Key.CompareTo(key);
                if (comp >= 0)
                    return comp * -1;

                comp = this.end.Value.Key.CompareTo(key);

                if (comp == -1)
                    return 1;
                else 
                    return 0;

            }

            public override string ToString()
            {
                if (start != null && end != null)
                {
                    return "IndexPage" + "{" + size + ", " + start.Value.Key + "->" + end.Value.Key + "}";
                }
                else
                {
                    return "IndexPage" + "{Empty}";
                }
            }
        }

        private class KeyNode
        {
            public string Key;
            public TVal value;

            public KeyNode(string Key, TVal value)
            {
                this.Key = Key;
                this.value = value;
            }
        }

    }

    public interface IIndexedDictionary<TVal> : IEnumerable<TVal>
    {
        IList<TVal> FindStartsWith(string substring);
        IList<TVal> FindExact(string key);
        IList<TVal> GetAllValues();
        void Put(string key, TVal item);
        int Count { get; }
    }
}

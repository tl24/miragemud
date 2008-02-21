using System;
using System.Collections.Generic;
using System.Text;

namespace Mirage.Core.Data
{
    /// <summary>
    /// Repository that exposes a single list usually loaded all at once from a  single table or file
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class AbstractSimpleRepository<T> : IEnumerable<T>
    {
        protected List<T> _items;

        public AbstractSimpleRepository()
        {
        }

        protected abstract List<T> Load();


        protected List<T> Items
        {
            get {
                if (_items == null) {
                    _items = Load();
                    if (_items == null)
                        _items = new List<T>();
                }
                return _items;
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            return Items.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}

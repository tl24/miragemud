using System;
using System.Collections.Generic;
using System.Text;

namespace Mirage.Util
{
    public sealed class SynchronizedQueue<T> : ISynchronizedQueue<T>
    {
        private Queue<T> _queue;

        public SynchronizedQueue(int initalCapicity)
        {
            _queue = new Queue<T>(initalCapicity);
        }

        public SynchronizedQueue()
        {
            _queue = new Queue<T>();
        }

        public SynchronizedQueue(IEnumerable<T> items)
        {
            _queue = new Queue<T>(items);            
        }

        #region ISynchronizedQueue<T> Members

        public void Clear()
        {
            lock (_queue)
            {
                _queue.Clear();
            }
        }

        public int Count
        {
            get { lock (_queue) { return _queue.Count; } }
        }

        public T Dequeue()
        {
            lock (_queue)
            {
                return _queue.Dequeue();
            }
        }

        public void Enqueue(T value)
        {
            lock (_queue)
            {
                _queue.Enqueue(value);
            }
        }

        public bool IsSynchronized
        {
            get { return true; }
        }

        public T Peek()
        {
            lock (_queue)
            {
                return _queue.Peek();
            }

        }

        public bool TryDequeue(out T value)
        {
            lock (_queue)
            {
                if (_queue.Count == 0)
                {
                    value = default(T);
                    return false;
                }
                else
                {
                    value = _queue.Dequeue();
                    return true;
                }
            }
        }

        public bool TryEnqueue(T value)
        {
            // we aren't bounded so this should always work
            Enqueue(value);
            return true;
        }

        public bool TryPeek(out T value)
        {
            lock (_queue)
            {
                if (_queue.Count == 0)
                {
                    value = default(T);
                    return false;
                }
                else
                {
                    value = _queue.Peek();
                    return true;
                }
            }
        }

        public T[] Values
        {
            get {
                lock (_queue)
                {
                    T[] _vals = new T[_queue.Count];
                    _queue.CopyTo(_vals, 0);
                    return _vals;
                }
            }
        }

        /// <summary>
        /// Copies the Queue elements to an existing one-dimensional Array,
        /// starting at the specified array index.
        /// </summary>
        /// <param name="array">
        /// The one-dimensional Array that is the destination
        /// of the elements copied from Queue.
        /// The Array must have zero-based indexing.</param>
        /// <param name="index">The zero-based index
        ///     in array at which copying begins.</param>
        public void CopyTo(Array array, int index)
        {
            T[] tmpArray = Values;
            tmpArray.CopyTo(array, index);
        }
        #endregion
    }
}

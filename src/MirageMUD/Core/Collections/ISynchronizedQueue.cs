using System;
namespace Mirage.Core.Collections
{
    /// <summary>
    /// Represents a synchronized thread-safe queue
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ISynchronizedQueue<T>
    {
        /// <summary>
        /// Removes all objects from the Queue.
        /// </summary>
        /// <remarks>
        /// Count is set to zero. Size does not change.
        /// </remarks>
        void Clear();

        /// <summary>
        /// Gets the number of elements contained in the Queue.
        /// </summary>
        int Count { get; }
        T Dequeue();
        void Enqueue(T value);
        bool IsSynchronized { get; }
        T Peek();
        bool TryDequeue(out T value);
        bool TryEnqueue(T value);
        bool TryPeek(out T value);
        T[] Values { get; }
        void CopyTo(Array array, int index);
    }
}

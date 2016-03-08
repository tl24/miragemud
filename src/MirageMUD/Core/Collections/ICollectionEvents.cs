using System;

namespace Mirage.Core.Collections
{
    public interface ICollectionEvents
    {
        event EventHandler<CollectionEventArgs> CollectionModified;

    }
}

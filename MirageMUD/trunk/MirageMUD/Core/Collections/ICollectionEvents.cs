using System;
using System.Collections.Generic;
using System.Text;

namespace Mirage.Core.Collections
{
    public interface ICollectionEvents
    {
        event EventHandler<CollectionEventArgs> CollectionModified;

    }
}

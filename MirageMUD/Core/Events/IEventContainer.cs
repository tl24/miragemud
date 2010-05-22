using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mirage.Core.Events
{
    public interface IEventContainer
    {
        string Name { get; }
        IEnumerable<IEvent> Events { get; }
    }
}

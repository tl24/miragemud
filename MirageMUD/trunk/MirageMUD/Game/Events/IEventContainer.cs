using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mirage.Game.Events
{
    public interface IEventContainer
    {
        string Name { get; }
        IEnumerable<IEvent> Events { get; }
    }
}

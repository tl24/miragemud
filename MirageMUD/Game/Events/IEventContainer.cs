using System.Collections.Generic;

namespace Mirage.Game.Events
{
    public interface IEventContainer
    {
        string Name { get; }
        IEnumerable<IEvent> Events { get; }
    }
}

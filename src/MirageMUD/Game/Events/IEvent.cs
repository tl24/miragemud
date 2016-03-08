using System.Collections.Generic;

namespace Mirage.Game.Events
{
    public interface IEvent
    {
        string Name { get; }
        EventCategory Category { get; }
        string Message { get; }
        IDictionary<string, object> Parameters { get; set; }
        int Strength { get; }
        EventSense Sense { get; }
    }
}

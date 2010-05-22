using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mirage.Core.Events
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

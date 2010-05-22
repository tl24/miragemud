using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mirage.Core.Events
{
    public class EventBase : IEvent
    {
        public EventBase()
        {
            this.Parameters = new Dictionary<string, object>(StringComparer.CurrentCultureIgnoreCase);
            this.Strength = EventStrength.Default;
        }
        public string Name { get; set; }

        public EventCategory Category { get; set; }

        public string Message { get; set; }

        public IDictionary<string, object> Parameters { get; set; }

        public int Strength { get; set; }

        public EventSense Sense { get; set; }
    }
}

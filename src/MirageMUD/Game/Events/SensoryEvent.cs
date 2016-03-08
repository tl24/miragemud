using System;
using System.Collections.Generic;

namespace Mirage.Game.Events
{
    public class SensoryEvent : IEventContainer
    {
        private List<IEvent> events;
        public SensoryEvent(string name) {
            this.Name = name;
            this.events = new List<IEvent>();
        }

        public string Name { get; private set; }

        public IEnumerable<IEvent> Events
        {
            get { return events; }
        }

        public void Add(IEvent @event)
        {
            if (@event == null)
                throw new ArgumentNullException("event");

            if (string.IsNullOrEmpty(@event.Name))
            {
                EventBase eb = (EventBase)@event;
                if (eb != null)
                    eb.Name = string.Format("{0}.{1:s}", this.Name, eb.Sense);
            }
            events.Add(@event);
        }
    }
}

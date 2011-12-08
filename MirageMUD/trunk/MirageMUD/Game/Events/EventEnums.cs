using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mirage.Game.Events
{
    public enum EventCategory
    {
        Other,
        Communication,
        Confirmation,
        Error
    }

    public enum EventSense
    {
        None,
        Sight,
        Sound,
        Smell,
        Taste,
        Touch
    }

    public static class EventStrength
    {
        public static readonly int Primary = 80;
        public static readonly int Medium = 50;
        public static readonly int Default = Medium;
        public static readonly int Weak = 10;
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using JsonExSerializer.TypeConversion;
using JsonExSerializer;

namespace Mirage.Core.Communication
{
    /// <summary>
    /// Defines a namespace for a message.  The namespace allows one to
    /// group logically related messages into a common subject area.  Namespaces
    /// are hierarchical.  All namespaces except for the root derive from a parent
    /// namespace.  The messages in a namespace are said to be more specific than
    /// those in the parent namespace.
    /// </summary>    
    public static class Namespaces
    {
        // NOTE: These must end in "/" in order to combine properly
        public static readonly Uri Root = new Uri("msg:/");
        public static readonly Uri System = new Uri(Root, "system/");
        public static readonly Uri SystemError = new Uri(System, "error/");
        public static readonly Uri Movement = new Uri(Root, "movement/");
        public static readonly Uri Communication = new Uri(Root, "communication/");
        public static readonly Uri Common = new Uri(Root, "common/");
        public static readonly Uri CommonError = new Uri(Common, "error/");

        public static readonly Uri Negotiation = new Uri(Root, "negotiation/");

        public static readonly Uri Authentication = new Uri(Negotiation, "authentication/");
        public static readonly Uri PlayerCreation = new Uri(Negotiation, "creation/");

        public static readonly Uri Builder = new Uri(Root, "builder/");

        public static readonly Uri Area = new Uri(Builder, "area/");

    }
}

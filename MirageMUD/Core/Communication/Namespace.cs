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
        public static readonly string Root = "";
        public static readonly string System = "system";
        public static readonly string SystemError = System + ".error";
        public static readonly string Movement = "movement";
        public static readonly string Communication = "communication";
        public static readonly string Common = "common";
        public static readonly string CommonError = Common + ".error";

        public static readonly string Negotiation = "negotiation";

        public static readonly string Authentication = Negotiation + ".authentication";
        public static readonly string PlayerCreation = Negotiation + ".creation";

        public static readonly string Builder = "builder";

        public static readonly string Area = Builder +  ".area";

    }
}

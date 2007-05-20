using System;
using System.Collections.Generic;
using System.Text;

namespace Shoop.Command
{
    /// <summary>
    /// Decorates a property to indicate that this argument indicates the actor
    /// or invoker of the command.
    /// </summary>
    [AttributeUsageAttribute(System.AttributeTargets.Parameter)]
    public class ActorAttribute : System.Attribute
    {
    }
}

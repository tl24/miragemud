using System;
using System.Collections.Generic;
using System.Text;

namespace Mirage.Game.Command
{
    /// <summary>
    /// Decorates a property to indicate that this argument indicates the actor
    /// or invoker of the command.
    /// </summary>
    [AttributeUsageAttribute(System.AttributeTargets.Parameter)]
    public class ActorAttribute : BaseArgumentAttribute
    {
    }
}

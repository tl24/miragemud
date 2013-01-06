using System;

namespace Mirage.Game.Command.Infrastructure
{
    /// <summary>
    /// Base attribute for attributes that are applied to command arguments
    /// </summary>
    [AttributeUsageAttribute(System.AttributeTargets.Parameter,AllowMultiple=false)]
    public abstract class CommandArgumentAttribute : System.Attribute
    {
    }
}

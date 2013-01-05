using System.Security.Principal;
using Mirage.Game.Communication;
using Mirage.Core.Messaging;

namespace Mirage.Game.World
{
    /// <summary>
    /// represents an object that is capable of invoking a command
    /// </summary>
    public interface IActor : IReceiveMessages
    {
        /// <summary>
        /// The level of the actor
        /// </summary>
        //TODO: Get rid of this, but the invoker currently checks this
        int Level { get; }

        /// <summary>
        /// Get security for executing commands
        /// </summary>
        IPrincipal Principal { get; }
    }
}

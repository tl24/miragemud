
using Mirage.Core.Command.ArgumentConversion;
using System.Collections.Generic;
namespace Mirage.Core.Command
{
    public abstract class CommandGroupBase : ICommandGroup
    {
        public abstract void InitializeArgumentHandlers(IEnumerable<Argument> arguments);
    }


    public interface ICommandGroup
    {
        /// <summary>
        /// Allows for a class to define its own argument conversion handlers for arguments to
        /// commands within the group
        /// </summary>
        /// <param name="arguments">the arguments to initialize</param>
        void InitializeArgumentHandlers(IEnumerable<Argument> arguments);
    }
}

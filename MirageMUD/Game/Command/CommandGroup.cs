using System;
using System.Collections.Generic;
using System.Text;

namespace Mirage.Game.Command
{
    public abstract class CommandGroupBase : ICommandGroup
    {
        public abstract void InitializeArgumentHandlers(ArgumentList arguments);
    }


    public interface ICommandGroup
    {
        /// <summary>
        /// Allows for a class to define its own argument conversion handlers for arguments to
        /// commands within the group
        /// </summary>
        /// <param name="arguments">the arguments to initialize</param>
        void InitializeArgumentHandlers(ArgumentList arguments);
    }
}

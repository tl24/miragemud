using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mirage.Core.Command.Guards;

namespace Mirage.Core.Command
{
    /// <summary>
    /// Base class for command restriction attributes.  A guard is produced for the command to implement the restriction
    /// </summary>
    public abstract class CommandRestrictionAttribute : System.Attribute
    {
        /// <summary>
        ///     Creates an instance of the attribute
        /// </summary>
        protected CommandRestrictionAttribute()
        {
        }

        /// <summary>
        /// Creates the guard for this restriction
        /// </summary>
        /// <returns></returns>
        public abstract ICommandGuard CreateGuard();
    }
}

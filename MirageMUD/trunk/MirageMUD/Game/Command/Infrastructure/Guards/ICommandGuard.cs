using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mirage.Game.World;

namespace Mirage.Game.Command.Infrastructure.Guards
{
    /// <summary>
    /// A guard for a command to prevent it from being executed by unqualified actors.  This should not be used for
    /// normal command validation, but is used during command selection so that for competing commands, uncallable
    /// commands are weeded out of the list of commands available.
    /// </summary>
    public interface ICommandGuard
    {
        bool IsSatisified(IActor actor);
    }
}

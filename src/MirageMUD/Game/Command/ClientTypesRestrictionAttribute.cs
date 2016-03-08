using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mirage.Game.Command.Guards;
using Mirage.Core.Command;
using Mirage.Core.Command.Guards;

namespace Mirage.Game.Command
{
    /// <summary>
    /// A command restriction that restricts the command to being executed by players using a certain client type
    /// </summary>
    [AttributeUsageAttribute(System.AttributeTargets.Class | System.AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    public class ClientTypesRestrictionAttribute : CommandRestrictionAttribute
    {
        public ClientTypesRestrictionAttribute(params Type[] clientTypes)
        {
            this.ClientTypes = clientTypes;
        }

        public Type[] ClientTypes { get; private set; }

        public override ICommandGuard CreateGuard()
        {
            return new ClientTypeGuard(ClientTypes);
        }
    }

}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mirage.Game.Command.Infrastructure.Guards;

namespace Mirage.Game.Command
{
    /// <summary>
    /// Restricts a command to players that has one of the security roles
    /// </summary>
    [AttributeUsageAttribute(System.AttributeTargets.Class | System.AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    public class RoleRestrictionAttribute : CommandRestrictionAttribute
    {
        public RoleRestrictionAttribute(params string[] roles)
        {
            this.Roles = roles;
        }

        public string[] Roles { get; private set; }

        public override Infrastructure.Guards.ICommandGuard CreateGuard()
        {
            return new RoleGuard(Roles);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mirage.Core.Command.Guards;

namespace Mirage.Core.Command
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

        public override Guards.ICommandGuard CreateGuard()
        {
            return new RoleGuard(Roles);
        }
    }
}

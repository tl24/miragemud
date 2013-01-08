using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Principal;

namespace Mirage.Core.Command.Guards
{
    public class RoleGuard : ICommandGuard
    {
        public RoleGuard(IEnumerable<string> roles)
        {
            if (roles == null)
                throw new ArgumentNullException("roles");
            this.Roles = roles.ToArray();
        }

        public string[] Roles { get; private set; }

        public bool IsSatisified(IActor actor)
        {
            if (Roles.Length > 0)
            {
                IPrincipal principal = actor.Principal;
                foreach (string role in Roles)
                {
                    if (principal.IsInRole(role))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}

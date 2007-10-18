using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Principal;

namespace Mirage.Core.Security
{
    /// <summary>
    /// Mud identity that identifies the login credentials of the specified entity
    /// </summary>
    public class MudIdentity : GenericIdentity
    {
        public MudIdentity(string name)
            : base(name)
        {
        }
    }
}

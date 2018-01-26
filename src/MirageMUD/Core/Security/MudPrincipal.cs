using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;

namespace Mirage.Core.Security
{
    /// <summary>
    /// A principal implementation for mud security.  This object will be used to test
    /// if the owner has the correct roles to execute privileged actions.
    /// </summary>
    /// <see cref="System.Security.Principal.IPrincipal"/>
    public class MudPrincipal : IPrincipal
    {
        /// <summary>
        /// A user with the administrator role can perform any function in the system,
        /// IsInRole will always return true.
        /// </summary>
        public const string AdministratorRole = "admin";

        private IIdentity _identity;
        private HashSet<string> _roles = new HashSet<string>(StringComparer.CurrentCultureIgnoreCase);

        /// <summary>
        /// Creates a mud principal with an identity and no roles.
        /// </summary>
        /// <param name="identity">the identity for the principal object</param>
        public MudPrincipal(IIdentity identity)
        {
            this._identity = identity;
        }

        /// <summary>
        /// Creates a mud principal with an identity and specified roles.
        /// </summary>
        /// <param name="identity">the identity for the principal object</param>
        /// <param name="roles">the roles for the principal</param>
        public MudPrincipal(IIdentity identity, IEnumerable<string> roles)
            : this(identity)
        {
            AddRoles(roles);
        }

        /// <summary>
        /// Returns the identity for this principal
        /// </summary>
        public IIdentity Identity
        {
            get { return _identity; }
        }

        /// <summary>
        /// tests to see if the principal has the given role
        /// </summary>
        /// <param name="role">the role to check</param>
        /// <returns>true if the principal is in the specified role</returns>
        public bool IsInRole(string role)
        {
            //admin can fulfill any role
            return _roles.Contains(role) || IsAdmin;
        }

        /// <summary>
        /// Adds a role to this principal
        /// </summary>
        /// <param name="roleName">the name of the role to add</param>
        public void AddRole(string roleName)
        {
            _roles.Add(roleName);
        }

        /// <summary>
        /// Adds the given roles to this principal.  It is ok to add roles
        /// that have already been added
        /// </summary>
        /// <param name="roles">the roles to add</param>
        public void AddRoles(IEnumerable<string> roles)
        {
            foreach (string role in roles)
                AddRole(role);
        }
        /// <summary>
        /// Removes a role from this principal
        /// </summary>
        /// <param name="roleName"></param>
        public void RemoveRole(string roleName)
        {
            _roles.Remove(roleName);
        }

        /// <summary>
        /// Clears all roles from this principal
        /// </summary>
        public void Clear()
        {
            _roles.Clear();
        }

        /// <summary>
        /// The roles for this principal
        /// </summary>
        public IEnumerable<string> Roles => _roles.ToList();

        /// <summary>
        /// Returns true if this user has the Administrator role
        /// </summary>
        public bool IsAdmin
        {
            get { return _roles.Contains(AdministratorRole); }
        }
    }
}

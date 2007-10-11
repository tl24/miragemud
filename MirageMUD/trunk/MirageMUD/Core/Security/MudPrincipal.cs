using System;
using System.Collections.Generic;
using System.Text;
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
        private IDictionary<string, bool> _roles = new Dictionary<string, bool>(StringComparer.CurrentCultureIgnoreCase);
        private bool _isAdmin = false;

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
        public MudPrincipal(IIdentity identity, string[] roles)
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
            if (IsAdmin)
                return true;

            bool allow = false;
            _roles.TryGetValue(role, out allow);
            return allow;
        }

        /// <summary>
        /// Adds a role to this principal
        /// </summary>
        /// <param name="roleName">the name of the role to add</param>
        public void AddRole(string roleName)
        {
            if (roleName.Equals(AdministratorRole, StringComparison.CurrentCultureIgnoreCase))
                _isAdmin = true;

            _roles[roleName] = true;
        }

        /// <summary>
        /// Adds the given roles to this principal.  It is ok to add roles
        /// that have already been added
        /// </summary>
        /// <param name="roles">the roles to add</param>
        public void AddRoles(string[] roles)
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
            if (roleName.Equals(AdministratorRole, StringComparison.CurrentCultureIgnoreCase))
                _isAdmin = false;
            _roles.Remove(roleName);
        }

        /// <summary>
        /// Clears all roles from this principal
        /// </summary>
        public void Clear()
        {
            _roles.Clear();
            _isAdmin = false;
        }

        /// <summary>
        /// The roles for this principal
        /// </summary>
        public string[] Roles
        {
            get { 
                string[] result = new string[_roles.Count];
                _roles.Keys.CopyTo(result, 0);
                return result;
            }
        }

        /// <summary>
        /// Returns true if this user has the Administrator role
        /// </summary>
        public bool IsAdmin
        {
            get { return _isAdmin; }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Principal;

namespace Mirage.Stock.Data
{
    public class MobPrincipal : IPrincipal
    {
        private IIdentity _identity;

        public MobPrincipal(string name)
        {
            _identity = new GenericIdentity(name);

        }
        #region IPrincipal Members

        public IIdentity Identity
        {
            get { return _identity; }
        }

        public bool IsInRole(string role)
        {
            if (role.Equals("mobile"))
                return true;
            else
                return false;
        }

        #endregion
    }

}

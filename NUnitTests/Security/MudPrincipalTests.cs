using System.Security.Principal;
using Mirage.Core.Security;
using NUnit.Framework;

namespace NUnitTests.Security
{
    [TestFixture]
    public class MudPrincipalTests
    {
        private IIdentity identity = null;

        [SetUp]
        public void TestSetup()
        {
            identity = new GenericIdentity("test");
        }

        [Test]
        public void TestNoDefaultRoles()
        {
            MudPrincipal p = new MudPrincipal(identity);
            Assert.AreEqual(0, p.Roles.Length, "should not have roles");
            Assert.IsFalse(p.IsInRole("default"), "IsInRole should return false when no roles");
        }

        [Test]
        public void TestAddRole()
        {
            MudPrincipal p = new MudPrincipal(identity);
            p.AddRole("A");
            Assert.IsTrue(p.IsInRole("A"));
            Assert.IsTrue(p.IsInRole("a"));
        }

        [Test]
        public void TestDefaultRoles()
        {
            MudPrincipal p = new MudPrincipal(identity, new string[]{ "A" });
            Assert.IsTrue(p.IsInRole("A"));
            Assert.IsTrue(p.IsInRole("a"));
        }

        [Test]
        public void TestRemoveRoles()
        {
            MudPrincipal p = new MudPrincipal(identity, new string[] { "A" });
            p.RemoveRole("a");
            Assert.IsFalse(p.IsInRole("A"));
            Assert.IsFalse(p.IsInRole("a"));
        }

        public void TestAdminRoles()
        {
            MudPrincipal p = new MudPrincipal(identity);
            p.AddRole(MudPrincipal.AdministratorRole);
            Assert.IsTrue(p.IsInRole("a"), "Admin should have all roles");
            Assert.IsTrue(p.IsAdmin, "IsAdmin should return true if user has AdministratorRole");
        }
    }
}

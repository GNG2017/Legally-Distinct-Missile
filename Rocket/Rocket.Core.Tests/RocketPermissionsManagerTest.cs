using NUnit.Framework;
using Rocket.API.Serialisation;
using Rocket.Core.Assets;
using Rocket.Core.Permissions;
using System.Collections.Generic;
using System.IO;

namespace Rocket.Core.Tests
{
    [TestFixture]
    public class RocketPermissionsHelperTest
    {
        private readonly RocketPermissionsHelper target = new RocketPermissionsHelper(new XMLFileAsset<RocketPermissions>(Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "Permissions.config.xml")));
        private readonly TestingPlayer p = new TestingPlayer();
        private readonly TestingPlayer p2 = new TestingPlayer("2");
        private readonly TestingPlayer p3 = new TestingPlayer("3", true);


        [Test]
        public void HasPermission()
        {
            Assert.IsTrue(target.HasPermission(p, new List<string>() { "p" }));
            Assert.IsFalse(target.HasPermission(p, new List<string>() { "p.reload" }));
            Assert.IsTrue(target.HasPermission(p, new List<string>() { "kit" }));
            Assert.IsTrue(target.HasPermission(p, new List<string>() { "kit.test" }));
            Assert.IsTrue(target.HasPermission(p, new List<string>() { "kit.test2" }));
            Assert.IsTrue(target.HasPermission(p, new List<string>() { "kit.test2.test3" }));

            Assert.IsTrue(target.HasPermission(p2, new List<string>() { "p" }));
            Assert.IsFalse(target.HasPermission(p2, new List<string>() { "p.reload" }));
            Assert.IsTrue(target.HasPermission(p2, new List<string>() { "kit" }));
            Assert.IsFalse(target.HasPermission(p2, new List<string>() { "kit.test" }));
            Assert.IsTrue(target.HasPermission(p2, new List<string>() { "kit.test2" }));

            Assert.IsTrue(target.HasPermission(p3, new List<string>() { "p" }) || p3.IsAdmin);
            Assert.IsTrue(target.HasPermission(p3, new List<string>() { "p.reload" }) || p3.IsAdmin);
            Assert.IsTrue(target.HasPermission(p3, new List<string>() { "kit" }) || p3.IsAdmin);
            Assert.IsTrue(target.HasPermission(p3, new List<string>() { "kit.test" }) || p3.IsAdmin);
            Assert.IsTrue(target.HasPermission(p3, new List<string>() { "kit.test2" }) || p3.IsAdmin);
            Assert.IsTrue(target.HasPermission(p3, new List<string>() { "kit.test2.test3" }) || p3.IsAdmin);
            Assert.IsTrue(target.HasPermission(p3, new List<string>() { "dummy" }) || p3.IsAdmin);
        }
    }
}

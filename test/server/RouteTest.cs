using System;
using NUnit.Framework;
using RestLib.Server;
using RestLib.Utils;

namespace RestLibTest.Server
{
    [TestFixture]
    public class RouteTest
    {
        Route target;
        
        [Test]
        public void RouteMatchesTest()
        {
            target = new Route("^/aab/.*$", HttpMethod.GET);
            Assert.That(target.Matches("get", "/aab/123a"));
        }
        
        [Test]
        public void RouteDoesNotMatchTest()
        {
            target = new Route("^/oow/\\d+$", HttpMethod.GET);
            Assert.That(target.Matches("get", "/oow/bcd"), Is.False);
        }
    }
}
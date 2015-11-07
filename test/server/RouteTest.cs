using System;
using System.Collections.Specialized;
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

        [Test]
        public void QueryStringTest()
        {
            CreateTarget("^/foo/(\\d+)\\??.*$");
            target.Matches("get", "/foo/123?k1=test&k2=8d,9b&k3=992&k1=rest");
            NameValueCollection queryString = target.QueryString;
            Assert.That(queryString.Count, Is.EqualTo(3));
            Assert.That(queryString["k1"], Is.EqualTo("test,rest"));
            Assert.That(queryString["k2"], Is.EqualTo("8d,9b"));
            Assert.That(queryString["k3"], Is.EqualTo("992"));
        }

        void CreateTarget(string path)
        {
            target = new Route(path, HttpMethod.GET);
        }
    }
}
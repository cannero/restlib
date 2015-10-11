using System;
using NUnit.Framework;
using RestLib.Server;

namespace RestLibTest.Server
{
    [TestFixture]
    public class RequestParserTest
    {

        [Test]
        public void GzipCanBeUsed()
        {
            Assert.That(RequestParser.GzipCanBeUsed(new string[] { "as", "gzip;q=0.8", "eiw;q=0.8" }), Is.True); 
        }

        [Test]
        public void GzipWithZeroQCannotBeUsed()
        {
            Assert.That(RequestParser.GzipCanBeUsed(new string[] { "as", "gzip;q=0", "eiw;q=0.8" }), Is.False); 
        }
    }
}
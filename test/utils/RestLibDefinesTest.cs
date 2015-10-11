using System;
using NUnit.Framework;
using RestLib.Server;
using RestLib.Utils;

namespace RestLibTest.Utils
{
    [TestFixture]
    public class RestLibDefinesTest
    {
        
        [Test]
        public void ContentTypeValue_ReturnsMetadata()
        {
            ContentType contType = ContentType.TextPlain;
            Assert.That(contType.GetValue(), Is.EqualTo("text/plain"));

            contType = ContentType.ApplicationJson;
            Assert.That(contType.GetValue(), Is.EqualTo("application/json"));
        }
    }
}
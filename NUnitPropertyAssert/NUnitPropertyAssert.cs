using System;
using System.Linq;
using NUnit.Framework;

namespace NUnitPropertyAssert
{
    [TestFixture]
    public class NUnitPropertyAssert
    {
        [Test]
        public void Test()
        {
            Assert.That(1, Is.EqualTo(1));
        }
    }
}
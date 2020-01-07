using Epok.Core.Utilities;
using NUnit.Framework;

namespace Epok.UnitTests.Core
{
    [TestFixture]
    public class EntityBaseTests
    {
        [Test]
        public void ShouldApplyOverriddenEquals()
        {
            var cookie = new GuineaPig("Cookie", 800, 37.7);
            var clone = cookie.DeepCopy();
            var other = new GuineaPig("Cookie", 800, 37.7);

            Assert.That(cookie.Equals(clone), Is.True);
            Assert.That(cookie.Equals(other), Is.False);
            Assert.That(cookie == clone, Is.True);
            Assert.That(cookie != other, Is.True);
        }

        [Test]
        public void ShouldApplyOverriddenToString()
        {
            var cookie = new GuineaPig("Cookie", 800, 37.7);

            Assert.That(cookie.ToString(), Is.EqualTo($"GuineaPig {cookie.Id} 'Cookie'"));
        }
    }
}

using Epok.Core.Utilities;
using NUnit.Framework;
using System.Reflection;

namespace Epok.UnitTests.Core
{
    [TestFixture]
    public class DeepCopyTests
    {
        [Test]
        public void ShouldPerformDeepCopy()
        {
            var pig = new GuineaPig("Cookie", 800, 37.7);
            var clone = pig.DeepCopy();

            Assert.That(pig.Name, Is.EqualTo(clone.Name));
            Assert.That(pig.Weight, Is.EqualTo(clone.Weight));

            var prop = typeof(GuineaPig).GetProperty("BodyTemp", BindingFlags.NonPublic | BindingFlags.Instance);
            Assert.That(prop?.GetValue(pig), Is.EqualTo(prop?.GetValue(clone)));
        }
    }
}

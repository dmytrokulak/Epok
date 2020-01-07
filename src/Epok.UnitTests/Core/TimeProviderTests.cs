using Epok.Core.Providers;
using NUnit.Framework;
using System;

namespace Epok.UnitTests.Core
{
    //todo:3 review DateTimeAdjustedTests; 
    [TestFixture]
    //9am - 6pm Mn-Fr
    //1pm - 2pm lunch break
    public class TimeProviderTests
    {
        private readonly ITimeProvider _timeProvider = new TimeProvider();

        [Test]
        public void CanAddAdjusted_Workdays_Success()
        {
            var ts = TimeSpan.FromHours(10);
            var dt = new DateTimeOffset(2020, 01, 02, 10, 0, 0, TimeSpan.Zero);

            var result = _timeProvider.Add(dt, ts);
            var expected = new DateTimeOffset(2020, 01, 03, 11, 0, 0, TimeSpan.Zero);
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void CanAddAdjusted_Weekend_Success()
        {
            var ts = TimeSpan.FromHours(10);
            var dt = new DateTimeOffset(2020, 01, 03, 10, 0, 0, TimeSpan.Zero);

            var result = _timeProvider.Add(dt, ts);
            var expected = new DateTimeOffset(2020, 01, 06, 11, 0, 0, TimeSpan.Zero);
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void CanAddAdjusted_MultipleDays_Success()
        {
            var ts = TimeSpan.FromHours(18);
            var dt = new DateTimeOffset(2020, 01, 02, 10, 0, 0, TimeSpan.Zero);

            var result = _timeProvider.Add(dt, ts);
            var expected = new DateTimeOffset(2020, 01, 06, 10, 0, 0, TimeSpan.Zero);
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void CanSubstructAdjusted_Workdays_Success()
        {
            var ts = TimeSpan.FromHours(10);
            var dt = new DateTimeOffset(2020, 01, 03, 12, 0, 0, TimeSpan.Zero);

            var result = _timeProvider.Subtract(dt, ts);
            var expected = new DateTimeOffset(2020, 01, 02, 10, 0, 0, TimeSpan.Zero);
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void CanSubstructAdjusted_Weekend_Success()
        {
            var ts = TimeSpan.FromHours(10);
            var dt = new DateTimeOffset(2020, 01, 06, 12, 0, 0, TimeSpan.Zero);

            var result = _timeProvider.Subtract(dt, ts);
            var expected = new DateTimeOffset(2020, 01, 03, 10, 0, 0, TimeSpan.Zero);
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void CanSubstructAdjusted_MultipleDays_Success()
        {
            var ts = TimeSpan.FromHours(18);
            var dt = new DateTimeOffset(2020, 01, 06, 12, 0, 0, TimeSpan.Zero);

            var result = _timeProvider.Subtract(dt, ts);
            var expected = new DateTimeOffset(2020, 01, 02, 11, 0, 0, TimeSpan.Zero);
            Assert.That(result, Is.EqualTo(expected));
        }
    }
}

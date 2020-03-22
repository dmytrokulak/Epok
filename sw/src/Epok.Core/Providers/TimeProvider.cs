using System;

namespace Epok.Core.Providers
{
    public class TimeProvider : ITimeProvider
    {
        public DateTimeOffset Add(DateTimeOffset dt, TimeSpan ts)
        {
            var lunchStart = new DateTimeOffset(dt.Year, dt.Month, dt.Day, 13, 0, 0, dt.Offset);
            var lunchStop = new DateTimeOffset(dt.Year, dt.Month, dt.Day, 14, 0, 0, dt.Offset);
            var officeStop = new DateTimeOffset(dt.Year, dt.Month, dt.Day, 18, 0, 0, dt.Offset);

            var subSpan = officeStop - dt;
            if (subSpan > ts)
            {
                if (dt < lunchStart && dt.Add(ts) > lunchStart)
                    return dt.Add(ts + (lunchStop - lunchStart));
                return dt.Add(ts);
            }

            var next = dt.DayOfWeek == DayOfWeek.Friday ? dt.AddDays(3) : dt.AddDays(1);
            var officeStart = new DateTimeOffset(next.Year, next.Month, next.Day, 9, 0, 0, next.Offset);

            return Add(officeStart, ts - subSpan);
        }

        public DateTimeOffset Subtract(DateTimeOffset dt, TimeSpan ts)
        {
            var officeStart = new DateTimeOffset(dt.Year, dt.Month, dt.Day, 9, 0, 0, dt.Offset);
            var lunchStart = new DateTimeOffset(dt.Year, dt.Month, dt.Day, 13, 0, 0, dt.Offset);
            var lunchStop = new DateTimeOffset(dt.Year, dt.Month, dt.Day, 14, 0, 0, dt.Offset);

            var subSpan = dt - officeStart;
            if (subSpan > ts)
            {
                if (dt > lunchStop && dt.Subtract(ts) < lunchStop)
                    return dt.Subtract(ts + (lunchStop - lunchStart));
                return dt.Subtract(ts);
            }

            var next = dt.DayOfWeek == DayOfWeek.Monday ? dt.AddDays(-3) : dt.AddDays(-1);
            var officeStop = new DateTimeOffset(next.Year, next.Month, next.Day, 18, 0, 0, next.Offset);

            return Subtract(officeStop, ts - subSpan);
        }
    }
}

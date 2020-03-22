using System;

namespace Epok.Core.Providers
{
    /// <summary>
    /// Takes into account office working time,
    /// holidays and weekends. 
    /// </summary>
    public interface ITimeProvider : ICrossCuttingProvider
    {
        /// <summary>
        /// Adjusted to working days and office hours.
        /// </summary>
        DateTimeOffset Add(DateTimeOffset dt, TimeSpan ts);

        /// <summary>
        /// Adjusted to working days and office hours.
        /// </summary>
        DateTimeOffset Subtract(DateTimeOffset dt, TimeSpan ts);
    }
}
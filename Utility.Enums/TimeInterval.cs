using System;


namespace Utility.Enums
{
    /// <summary>
    /// Time intervals ordered from smallest to largest
    /// </summary>

    public enum TimeInterval
    {
        Attosecond = 1,
        Femtosecond = 2,
        Picosecond = 3,
        Nanosecond = 4,
        Microsecond = 5,
        Millisecond = 6,
        Second = 7,
        Minute = 8,
        Hour = 9,
        Day = 10,
        Week = 11,
        Fortnight = 12,
        Month = 13,
        Quarter = 14,
        Year = 15,
        Decade = 16,
        Century = 17,
        Millennium = 18,
        Eon = 19,
    }



    /// <summary>
    /// Extension methods for working with time intervals
    /// </summary>
    public static class TimeIntervalExtensions
    {
        /// <summary>
        /// Returns the TimeSpan representation  of a <see cref="TimeInterval"/>
        /// </summary>
        public static TimeSpan? ToTimeSpan(this TimeInterval interval) => interval switch
        {
            TimeInterval.Nanosecond => throw new NotImplementedException(),
            TimeInterval.Microsecond => TimeSpan.FromTicks(10),
            TimeInterval.Millisecond => TimeSpan.FromMilliseconds(1),
            TimeInterval.Second => TimeSpan.FromSeconds(1),
            TimeInterval.Minute => TimeSpan.FromMinutes(1),
            TimeInterval.Hour => TimeSpan.FromHours(1),
            TimeInterval.Day => TimeSpan.FromDays(1),
            TimeInterval.Week => TimeSpan.FromDays(7),
            TimeInterval.Attosecond => throw new NotImplementedException(),
            TimeInterval.Femtosecond => throw new NotImplementedException(),
            TimeInterval.Picosecond => throw new NotImplementedException(),
            TimeInterval.Fortnight => TimeSpan.FromDays(14),
            TimeInterval.Month => throw new NotImplementedException(),
            TimeInterval.Quarter => throw new NotImplementedException(),
            TimeInterval.Year => throw new NotImplementedException(),
            TimeInterval.Decade => throw new NotImplementedException(),
            TimeInterval.Century => throw new NotImplementedException(),
            TimeInterval.Millennium => throw new NotImplementedException(),
            TimeInterval.Eon => throw new NotImplementedException(),
            _ => null // Month, Quarter, Year, etc. don't have fixed TimeSpan values
        };

        /// <summary>
        /// Returns an approximate duration in seconds of a <see cref="TimeInterval"/>
        /// </summary>
        public static double ToSeconds(this TimeInterval interval) => interval switch
        {
            TimeInterval.Nanosecond => 0.000000001,
            TimeInterval.Microsecond => 0.000001,
            TimeInterval.Millisecond => 0.001,
            TimeInterval.Second => 1,
            TimeInterval.Minute => 60,
            TimeInterval.Hour => 3600,
            TimeInterval.Day => 86400,
            TimeInterval.Week => 604800,
            TimeInterval.Month => 2629746, // Average month (30.44 days)
            TimeInterval.Quarter => 7889238, // 3 months
            TimeInterval.Year => 31556952, // Average year (365.25 days)
            TimeInterval.Decade => 315569520,
            TimeInterval.Century => 3155695200,
            TimeInterval.Millennium => 31556952000,
            _ => throw new ArgumentOutOfRangeException(nameof(interval))
        };

        /// <summary>
        /// Returns a human-readable string representation of a <see cref="TimeInterval"/>
        /// </summary>
        public static string ToUnit(this TimeInterval interval) => interval switch
        {
            TimeInterval.Nanosecond => "ns",
            TimeInterval.Microsecond => "μs",
            TimeInterval.Millisecond => "ms",
            TimeInterval.Second => "s",
            TimeInterval.Minute => "m",
            TimeInterval.Hour => "h",
            _ => interval.ToString()
        };


    }
}
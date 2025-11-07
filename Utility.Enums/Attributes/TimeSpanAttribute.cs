using System;

namespace Utility.Enums.Attributes
{
    public class TimeSpanAttribute(string name, double interval) : Attribute
    {
        private static readonly Lazy<Type> type = new(() => typeof(TimeSpan));
        private readonly Lazy<TimeSpan> timeSpan = new(() => (TimeSpan)type.Value.GetMethod(name).Invoke(null, [interval]));

        public string Name { get; } = name;
        public double Interval { get; } = interval;

        public TimeSpan TimeSpan => timeSpan.Value;
    }
}
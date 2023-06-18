
using System;

namespace Utility.Tasks
{
    public struct TimedValue<T> : ITimed
    {
        public TimedValue(T value, DateTime start, DateTime finish)
        {
            Value = value;
            Finish = finish;
            Start = start;
        }

        public T Value { get; }

        public DateTime Start { get; }

        public DateTime Finish { get; }

    }
}



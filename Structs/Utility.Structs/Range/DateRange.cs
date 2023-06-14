

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Utility.Structs
{

    /// <summary>
    ///  Represents a range of dates.
    ///  <a href="https://gist.github.com/crmorgan/5de0359b31555a80d9d8"></a>
    ///  crmorgan/DateRange.cs
    /// </summary>
    public readonly struct DateRange
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="DateRange" /> structure to the specified start and end date.
        /// </summary>
        /// <param name="startDate">A string that contains that first date in the date range.</param>
        /// <param name="endDate">A string that contains the last date in the date range.</param>
        /// <exception cref="System.ArgumentNullException">
        ///		endDate or startDate are <c>null</c>.
        /// </exception>
        /// <exception cref="System.FormatException">
        ///     endDate or startDate do not contain a vaild string representation of a date and time.
        /// </exception>
        /// <exception cref="System.ArgumentException">
        ///		endDate is not greater than or equal to startDate
        /// </exception>
        public DateRange(string startDate, string endDate) : this()
        {
            if (string.IsNullOrWhiteSpace(startDate))
            {
                throw new ArgumentNullException("startDate");
            }

            if (string.IsNullOrWhiteSpace(endDate))
            {
                throw new ArgumentNullException("endDate");
            }

            Start = DateTime.Parse(startDate);
            End = DateTime.Parse(endDate);

            if (End < Start)
            {
                throw new ArgumentException("endDate must be greater than or equal to startDate");
            }
        }

        public DateRange(DateTime startDate, DateTime endDate) : this()
        {


            Start = startDate;
            End = endDate;

            if (End < Start)
            {
                throw new ArgumentException("endDate must be greater than or equal to startDate");
            }
        }
        /// <summary>
        ///     Gets the start date compthisnt of the date range.
        /// </summary>
        public DateTime Start { get;  }


        /// <summary>
        ///     Gets the end date compthisnt of the date range.
        /// </summary>
        public DateTime End { get;  }

        /// <summary>
        ///     Gets a collection of the dates in the date range.
        /// </summary>
        public IList<DateTime> Dates
        {
            get
            {
                var startDate = Start;

                return Enumerable.Range(0, Days)
                    .Select(offset => startDate.AddDays(offset))
                    .ToList();
            }
        }

        /// <summary>
        ///     Gets the number of whole days in the date range.
        /// </summary>
        public int Days
        {
            get { return (End - Start).Days + 1; }
        }


        public DateRange GetoverLapWith(DateRange other)
        {
            if (this.HasPartialOverLapWith(other))
                if (this.DoesStartBeforeStartOf(other))
                    return new DateRange(other.Start, this.End);
                else
                    return new DateRange(this.Start, other.End);
            else if (this.HasFullOverLapWith(other))
                if (this.DoesStartBeforeStartOf(other))
                    return new DateRange(other.Start, other.End);
                else
                    return new DateRange(this.Start, this.End);
            else
                return default(DateRange);
        }



        public bool HasOverLapWith(DateRange other)
        {
            return !this.IsFullyAfter(other) && !other.IsFullyAfter(other);
        }

        public bool HasPartialOverLapWith(DateRange other)
        {
            return

            (this.DoesStartBeforeStartOf(other) && this.DoesEndBeforeEndOf(other) && other.DoesStartBeforeEndOf(this))
            ||
            (other.DoesStartBeforeStartOf(this) && other.DoesEndBeforeEndOf(this) && this.DoesStartBeforeEndOf(other));
        }

        public bool HasFullOverLapWith(DateRange other)
        {
            return (this.IsFullyWithin(other) || other.IsFullyWithin(this));
        }

        public bool IsFullyWithin(DateRange other)
        {
            return (this.DoesStartBeforeStartOf(this) && this.DoesEndBeforeStartOf(other));
        }

        private bool IsFullyAfter(DateRange other)
        {
            return this.Start > other.GetNullSafeEnd();
        }

        private bool DoesStartBeforeStartOf(DateRange other)
        {
            return this.Start <= other.Start;
        }

        private bool DoesStartBeforeEndOf(DateRange other)
        {
            return this.Start < other.GetNullSafeEnd();
        }

        private bool DoesEndBeforeEndOf(DateRange other)
        {
            return this.GetNullSafeEnd() <= other.GetNullSafeEnd();
        }

        private bool DoesEndBeforeStartOf(DateRange other)
        {
            return this.GetNullSafeEnd() < other.Start;
        }

        public DateTime GetNullSafeEnd() => End == default ? DateTime.MaxValue : this.End;

    }



    public static class Grouper
    {
        public static IEnumerable<IGrouping<DateRange, T>> GroupByDateRange<T>(this IOrderedEnumerable<T> enumerable, TimeSpan timeSpan, Func<T, DateTime> predicate)
        {
            Grouping<T> grouping = null;
            foreach (var (a, dt) in from b in enumerable select (b, predicate.Invoke(b)))
            {
                if (grouping == null || dt > grouping.Key.End)
                    yield return grouping = new Grouping<T>(new DateRange(dt, dt + timeSpan), a);
                else
                    grouping.Add(a);
            }
        }

        private class Grouping<T> : IGrouping<DateRange, T>
        {
            private readonly List<T> elements = new List<T>();

            public DateRange Key { get; }

            public Grouping(DateRange key) => Key = key;

            public Grouping(DateRange key, T element) : this(key) => Add(element);

            public void Add(T element) => elements.Add(element);

            public IEnumerator<T> GetEnumerator() => this.elements.GetEnumerator();

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }
    }
}



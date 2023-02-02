using System;

namespace Utility.Structs.FileSystem
{
    /// <summary>
    /// <a href="https://stackoverflow.com/questions/50634058/concatenate-readonlyspanchar"></a>
    /// </summary>
    public static class ReadOnlySpanHelper
    {
        public static bool IsAllLetters(this ReadOnlySpan<char> span)
        {
            foreach (char c in span)
            {
                if (!char.IsLetter(c))
                    return false;
            }
            return true;
        }

        /// <summary>
        /// <a href="http://stackoverflow.com/a/22565605"></a>  with some adaptions
        /// </summary>
        /// <param name="source"></param>
        /// <param name="oldValue"></param>
        /// <param name="newValue"></param>
        /// <param name="comparisonType"></param>
        /// <returns></returns>
        public static ReadOnlySpan<char> Replace(this ReadOnlySpan<char> source, ReadOnlySpan<char> oldValue, ReadOnlySpan<char> newValue, StringComparison comparisonType = StringComparison.OrdinalIgnoreCase)
        {
            if (oldValue.Length == 0)
            {
                throw new ArgumentNullException(nameof(oldValue));
            }
            if (source.Length == 0)
            {
                return source;
            }

            ReadOnlySpan<char> result = new ReadOnlySpan<char>();
            int startingPos = 0;
            int nextMatch;
            while ((nextMatch = source[startingPos..].IndexOf(oldValue, comparisonType)) > -1)
            {
                result = result.Concat(source[startingPos..(nextMatch + startingPos)], newValue);
                startingPos = startingPos + nextMatch + oldValue.Length;
            }

            result = result.Concat(source[startingPos..]);
            return result;
        }

        public static ReadOnlySpan<T> Concat<T>(this ReadOnlySpan<T> span0, ReadOnlySpan<T> span1)
        {
            var resultSpan = new T[span0.Length + span1.Length].AsSpan();
            span0.CopyTo(resultSpan);
            var from = span0.Length;
            span1.CopyTo(resultSpan[from..]);
            return resultSpan;
        }

        public static ReadOnlySpan<T> Concat<T>(this ReadOnlySpan<T> span0, ReadOnlySpan<T> span1, ReadOnlySpan<T> span2)
        {
            var resultSpan = new T[span0.Length + span1.Length + span2.Length].AsSpan();
            span0.CopyTo(resultSpan);
            var from = span0.Length;
            span1.CopyTo(resultSpan[from..]);
            from += span1.Length;
            span2.CopyTo(resultSpan[from..]);
            return resultSpan;
        }

        public static ReadOnlySpan<T> Concat<T>(this ReadOnlySpan<T> span0, ReadOnlySpan<T> span1, ReadOnlySpan<T> span2, ReadOnlySpan<T> span3)
        {
            var resultSpan = new T[span0.Length + span1.Length + span2.Length + span3.Length].AsSpan();
            span0.CopyTo(resultSpan);
            var from = span0.Length;
            span1.CopyTo(resultSpan[from..]);
            from += span1.Length;
            span2.CopyTo(resultSpan[from..]);
            from += span2.Length;
            span3.CopyTo(resultSpan[from..]);
            return resultSpan;
        }

        public static ReadOnlySpan<T> Concat<T>(this ReadOnlySpan<T> span0, ReadOnlySpan<T> span1, ReadOnlySpan<T> span2, ReadOnlySpan<T> span3, ReadOnlySpan<T> span4)
        {
            var result = new T[span0.Length + span1.Length + span2.Length + span3.Length + span4.Length];
            var resultSpan = result.AsSpan();
            span0.CopyTo(resultSpan);
            var from = span0.Length;
            span1.CopyTo(resultSpan[from..]);
            from += span1.Length;
            span2.CopyTo(resultSpan[from..]);
            from += span2.Length;
            span3.CopyTo(resultSpan[from..]);
            from += span3.Length;
            span4.CopyTo(resultSpan[from..]);
            return resultSpan;
        }

        public static ReadOnlySpan<T> Concat<T>(this ReadOnlySpan<T> span0, ReadOnlySpan<T> span1, ReadOnlySpan<T> span2, ReadOnlySpan<T> span3, ReadOnlySpan<T> span4, ReadOnlySpan<T> span5)
        {
            var result = new T[span0.Length + span1.Length + span2.Length + span3.Length + span4.Length + span5.Length];
            var resultSpan = result.AsSpan();
            span0.CopyTo(resultSpan);
            var from = span0.Length;
            span1.CopyTo(resultSpan[from..]);
            from += span1.Length;
            span2.CopyTo(resultSpan[from..]);
            from += span2.Length;
            span3.CopyTo(resultSpan[from..]);
            from += span3.Length;
            span4.CopyTo(resultSpan[from..]);
            from += span4.Length;
            span5.CopyTo(resultSpan[from..]);
            return resultSpan;
        }
    }
}
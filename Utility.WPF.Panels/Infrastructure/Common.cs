using System;
using System.Collections.Generic;
using System.Linq;

namespace Utility.WPF.Demo.Panels
{
    public static class Common
    {
        public static OptionalArray<T> ToOptionalArray<T>(this IEnumerable<T> values) where T : struct
        {
            return new OptionalArray<T>(values);
        }

        public static double Max(double first, params double[] other) => Max(first, (a, b) => a.CompareTo(b), other);

        public static int Max(int first, params int[] other) => Max(first, (a, b) => a.CompareTo(b), other);

        public static double? Max(double? first, params double?[] other) =>
              Max(first,
                (a, b) =>
                {
                    return a.HasValue && b.HasValue ? a.Value.CompareTo(b.Value) : a.HasValue ? 1 : b.HasValue ? -1 : 0;
                }, other);

        public static int? Max(int? first, params int?[] other) =>
            Max(first,
                (a, b) =>
                {
                    return a.HasValue && b.HasValue ? a.Value.CompareTo(b.Value) : a.HasValue ? 1 : b.HasValue ? -1 : 0;
                }, other);

        public static T Max<T>(T first, Func<T, T, int> compare, params T[] other)
        {
            var max = first;
            foreach (var x in other)
            {
                if (compare(x, max) > 0)
                    max = x;
            }
            return max;
        }
    }

    public class OptionalArray<T> where T : struct
    {
        private readonly T[] list;

        public OptionalArray(IEnumerable<T> values = null)
        {
            list = values.ToArray();
        }

        public T? this[int index]
        {
            get
            {
                return list.Length > index ? list[index] : default;
            }
        }
    }
}
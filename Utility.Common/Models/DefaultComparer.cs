using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Utility.Common
{
    public enum SortOrder
    {
        Ascending, Descending
    }

    public class DefaultComparer<T, R> : IComparer<T> where R : IComparable<R>
    {
        private readonly Func<T, R> func;
        private readonly SortOrder sortOrder;

        public DefaultComparer(Func<T, R> func, SortOrder sortOrder)
        {
            this.func = func;
            this.sortOrder = sortOrder;
        }

        public int Compare([AllowNull] T x, [AllowNull] T y)
        {
            if (sortOrder == SortOrder.Ascending)
                return func(x).CompareTo(func(y));
            else
                return func(y).CompareTo(func(x));

        }
    }

    public class Comparer
    {
        public static DefaultComparer<T, R> Create<T, R>(Func<T, R> func, SortOrder sortOrder = SortOrder.Ascending) where R : IComparable<R>
        {
            return new DefaultComparer<T, R>(func, sortOrder);
        }

        public static IComparer<T> Create<T>(SortOrder sortOrder = SortOrder.Ascending) where T : IComparable<T>
        {
            return new DefaultComparer<T, T>(a => a, sortOrder);
        }
    }
}
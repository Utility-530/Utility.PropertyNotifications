using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Utility
{
    public class LambdaEqualityComparer<TItem>: IEqualityComparer<TItem>
    {
        private readonly Func<TItem, TItem, bool> func;
        private readonly Func<TItem, int> hashCode;

        public LambdaEqualityComparer(Func<TItem,TItem, bool> func, Func<TItem, int> hashCode)
        {
            this.func = func;
            this.hashCode = hashCode;
        }

        public bool Equals(TItem? x, TItem? y)
        {
            return func(x, y);
        }

        public int GetHashCode([DisallowNull] TItem obj)
        {
            return hashCode(obj);
        }

        public static LambdaEqualityComparer<TItem> Create(Func<TItem, TItem, bool> func, Func<TItem, int> hashCode)
        {
            return new LambdaEqualityComparer<TItem>(func, hashCode);
        }
    }
}

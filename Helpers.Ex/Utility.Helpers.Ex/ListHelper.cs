using System;
using System.Collections.Generic;
using System.Linq;
using ObjectsComparer;

namespace Utility.Helpers.Ex;

public static class ListHelper
{
    public static void InsertInOrder<T>(this IList<T> collection, T item) where T : IComparable<T>
    {
        if (collection.Count == 0)
        {
            collection.Add(item);
            return;
        }

        if (collection.Count > 0 && collection[^1].CompareTo(item) <= 0)
        {
            collection.Add(item);
            return;
        }

        if (collection[0].CompareTo(item) >= 0)
        {
            collection.Insert(0, item);
            return;
        }
        int index = collection.BinarySearchIndex(item);
        if (index < 0)
            index = ~index;

        collection.Insert(index, item);
    }

    public static int BinarySearchIndex<T>(this IList<T> list, T item) where T : IComparable<T>
    {
        int lo = 0;
        int hi = list.Count - 1;

        while (lo <= hi)
        {
            int mid = lo + ((hi - lo) / 2);
            int cmp = list[mid].CompareTo(item);

            if (cmp == 0)
                return mid;

            if (cmp < 0)
                lo = mid + 1;
            else
                hi = mid - 1;
        }

        // return the bitwise complement of the insert index
        return ~lo;
    }

    public static void InsertInOrderIfMissing<T>(this IList<T> collection, params T[] set)
        where T : IEquatable<T>, IComparable<T>
    {
        var comparer = new ObjectsComparer.Comparer<T>(new ComparisonSettings { });

        //var dif2f = Netsoft.Diff.Differences.Between(set, collection.ToArray());
        foreach (var item in set)
        {
            if (collection.Contains(item))
            {
                var single = collection.Single(a => a.Equals(item));
                // var diff = comparer.CalculateDifferences(item, single);
                var equal = TextJsonHelper.Compare(single, item);
                if (equal)
                {
                    continue;
                }
                collection.Remove(item);
            }

            collection.InsertInOrder(item);
        }
    }
}
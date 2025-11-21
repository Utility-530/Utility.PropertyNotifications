using System;
using System.Collections.Generic;
using System.Linq;
using ObjectsComparer;
using Utility.Helpers.Generic;

namespace Utility.Helpers.Ex;

public static class ListHelper
{

    public static void InsertInOrderIfMissing<T>(this IList<T> collection, params T[] set)
        where T : IEquatable<T>, IComparable<T>
    {
        //var comparer = new ObjectsComparer.Comparer<T>(new ComparisonSettings { });

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
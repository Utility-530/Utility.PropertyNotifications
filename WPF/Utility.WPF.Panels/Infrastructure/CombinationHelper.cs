using System.Collections.Generic;
using System.Linq;

namespace Utility.WPF.Demo.Panels
{
    public static class CombinationHelper
    {
        public static IEnumerable<(T, T)> SelectCombinations<T>(this IList<T> coordinates)
        {
            int coordinatesIndex = coordinates.Count - 1;
            while (coordinatesIndex >= 0)
            {
                var last = coordinates[coordinatesIndex];
                yield return (last, last);

                foreach (var c in coordinates.Take(coordinatesIndex))
                {
                    yield return (last, c);
                    yield return (c, last);
                }
                coordinatesIndex--;
                SelectCombinations(coordinates);
            }
        }

        public static IEnumerable<IEnumerable<T>> SelectSetCombinations<T>(this IEnumerable<IEnumerable<T>> setOfSets)
        {
            using var enumr = setOfSets.GetEnumerator();
            enumr.MoveNext();
            IEnumerable<IEnumerable<T>> combinations = enumr.Current.Select(a => new[] { a });
            while (enumr.MoveNext())
                combinations = AddToCollection(combinations, enumr.Current);

            return combinations;

            static IEnumerable<IEnumerable<T>> AddToCollection<T>(IEnumerable<IEnumerable<T>> elementSets, IEnumerable<T> newElements)
            {
                foreach (var elements in elementSets)
                    foreach (var newElement in newElements)
                        yield return elements.Concat(new[] { newElement });
            }
        }
    }
}
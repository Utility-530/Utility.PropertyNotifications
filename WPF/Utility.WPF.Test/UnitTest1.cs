using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace Utility.WPF.Test
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Test1()
        {
            var setOfSets = Enumerable.Range(0, 4).Select((a, i) => Enumerable.Range(0, 5 - i).ToArray()).ToArray();
            var combos = SelectCombinations(setOfSets).ToArray();
            Assert.Pass();
        }

        private static IEnumerable<IEnumerable<T>> SelectCombinations<T>(IEnumerable<IList<T>> setOfSets)
        {
            using var enumerable = setOfSets.GetEnumerator();
            enumerable.MoveNext();
            IEnumerable<IEnumerable<T>> combinations = enumerable.Current.Select(a => new[] { a });
            while (enumerable.MoveNext())
                combinations = AddToCollection(combinations, enumerable.Current);

            return combinations;

            static IEnumerable<IEnumerable<TT>> AddToCollection<TT>(IEnumerable<IEnumerable<TT>> elementSets,
                IEnumerable<TT> newElements)
            {
                var enumerable = newElements as TT[] ?? newElements.ToArray();
                foreach (var elements in elementSets)
                {
                    var array = elements as TT[] ?? elements.ToArray();

                    foreach (var newElement in enumerable)
                        yield return array.Concat(new[] { newElement });
                }
            }
        }
    }
}
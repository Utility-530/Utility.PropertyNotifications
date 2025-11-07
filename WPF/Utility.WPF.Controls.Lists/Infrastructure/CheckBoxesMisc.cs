using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls.Primitives;
using Utility.Helpers;
using Utility.WPF.Models;

namespace Utility.WPF.Controls.Lists.Infrastructure
{
    internal class DifferenceHelper : IDifferenceHelper
    {
        private readonly Selector selector;
        private Dictionary<object, bool?>? dictionary;

        public DifferenceHelper(Selector selector)
        {
            this.selector = selector;
        }

        public IEnumerable<CheckedRoutedEventArgs.ChangedItem> Get
        {
            get
            {
                var tempDictionary = selector.ToDictionary();
                var differences = Differences().ToArray();
                dictionary = tempDictionary;
                return differences;

                IEnumerable<CheckedRoutedEventArgs.ChangedItem> Differences()
                {
                    if (dictionary == null)
                    {
                        return Array.Empty<CheckedRoutedEventArgs.ChangedItem>();
                    }
                    return dictionary.Differences(tempDictionary)
                        .Select(a => new CheckedRoutedEventArgs.ChangedItem(a.key, a.one, a.two));
                }
            }
        }
    }
}
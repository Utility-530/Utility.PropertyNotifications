using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace Utility.WPF.Helpers
{
    public static class SelectorHelper
    {
        public static IEnumerable<T> ItemsOfType<T>(this System.Windows.Controls.Primitives.Selector selector) where T : DependencyObject
        {
            return selector.Items
                .Cast<object>()
                .Select(a => selector.ItemContainerGenerator.ContainerFromItem(a))
                .Cast<T>();
        }
    }
}
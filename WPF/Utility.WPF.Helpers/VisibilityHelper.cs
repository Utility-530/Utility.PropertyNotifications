using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Utility.WPF
{
    public class VisibilityHelper
    {
        public static bool IsUserVisible(FrameworkElement element, FrameworkElement container)
        {
            if (!element.IsVisible)
                return false;

            Rect bounds =
                element.TransformToAncestor(container).TransformBounds(new Rect(0.0, 0.0, element.ActualWidth, element.ActualHeight));
            var rect = new Rect(0.0, 0.0, container.ActualWidth, container.ActualHeight);
            return rect.Contains(bounds.TopLeft) || rect.Contains(bounds.BottomRight);
        }

        // Afterwards you can loop through the ListBoxItems and use that test to determine which are visible.

        public static List<object> GetVisibleItemsFromListbox(ListBox listBox, FrameworkElement parentToTestVisibility)
        {
            var items = new List<object>();

            foreach (var item in listBox.Items)
            {
                if (IsUserVisible((ListBoxItem)listBox.ItemContainerGenerator.ContainerFromItem(item), parentToTestVisibility))
                {
                    items.Add(item);
                }
                else if (items.Any())
                {
                    break;
                }
            }

            return items;
        }
    }
}
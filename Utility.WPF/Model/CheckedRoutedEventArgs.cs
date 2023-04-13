using System.Collections.Generic;
using System.Windows;

namespace Utility.WPF.Controls.Lists.Infrastructure
{
    public class CheckedRoutedEventArgs : RoutedEventArgs
    {
        public CheckedRoutedEventArgs(RoutedEvent routedEvent,
            object source,
            ICollection<ChangedItem> dictionary)
            : base(routedEvent, source)
        {
            Dictionary = dictionary;
        }

        public ICollection<ChangedItem> Dictionary { get; }

        public record struct ChangedItem(object Key, bool? Old, bool? New);
    }
}

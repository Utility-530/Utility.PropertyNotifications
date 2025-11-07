using System.Collections.Generic;
using System.Windows;

namespace Utility.WPF.Models
{
    public class SelectedRoutedEventArgs : RoutedEventArgs
    {
        public SelectedRoutedEventArgs(RoutedEvent routedEvent,
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
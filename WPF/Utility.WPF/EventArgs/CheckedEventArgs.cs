using System.Windows;

namespace Utility.WPF
{
    public class CheckedEventArgs : RoutedEventArgs
    {
        public CheckedEventArgs(bool isChecked, RoutedEvent routedEvent, object source) : base(routedEvent, source)
        {
            IsChecked = isChecked;
        }

        public bool IsChecked { get; }
    }

    public delegate void CheckedRoutedEventHandler(object sender, CheckedEventArgs e);


}

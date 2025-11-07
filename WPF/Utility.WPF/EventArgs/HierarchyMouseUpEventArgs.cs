using System.Windows;

namespace Utility.WPF
{
    public class HierarchyMouseUpEventArgs : RoutedEventArgs
    {
        public HierarchyMouseUpEventArgs(RoutedEvent routedEvent, object source) : base(routedEvent, source)
        {
        }
    }

    public delegate void HierarchyMouseUpEventHandler(object sender, HierarchyMouseUpEventArgs e);
}
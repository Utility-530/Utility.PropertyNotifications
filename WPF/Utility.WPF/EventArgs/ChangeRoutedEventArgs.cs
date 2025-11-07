using System.Windows;

namespace Utility.WPF
{
    public class ChangeRoutedEventArgs : RoutedEventArgs
    {
        public ChangeRoutedEventArgs(Utility.Changes.Type type, object instance, RoutedEvent routedEvent, object source) : base(routedEvent, source)
        {
            Type = type;
            Instance = instance;
        }

        public Changes.Type Type { get; }
        public object Instance { get; }
    }

    public delegate void ChangeRoutedEventHandler(object sender, ChangeRoutedEventArgs e);
}
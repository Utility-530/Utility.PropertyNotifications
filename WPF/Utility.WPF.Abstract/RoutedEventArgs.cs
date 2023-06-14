using System.Windows;

namespace Utility.WPF.Abstract
{
    public delegate void RoutedEventHandler<TR>(object sender, RoutedEventArgs<TR> e);

    public class RoutedEventArgs<TEventArgs> : RoutedEventArgs
    {
        public RoutedEventArgs(TEventArgs Target, RoutedEvent routedEvent) : base(routedEvent)
        {
            Value = Target;
        }

        public TEventArgs Value { get; set; }
    }
}
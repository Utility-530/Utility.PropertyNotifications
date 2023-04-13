using System.Collections.Generic;
using System.Windows;

namespace Utility.WPF.Events
{
    public delegate void CheckedChangedEventHandler(object sender, CheckedChangedEventArgs e);

    public class CheckedChangedEventArgs : RoutedEventArgs
    {
        public CheckedChangedEventArgs(RoutedEvent routedEvent, object source,
            IReadOnlyCollection<object> @checked,
            IReadOnlyCollection<object> unChecked) : base(routedEvent, source)
        {
            Checked = @checked;
            UnChecked = unChecked;
        }

        public IReadOnlyCollection<object> Checked { get; }
        public IReadOnlyCollection<object> UnChecked { get; }
    }
}
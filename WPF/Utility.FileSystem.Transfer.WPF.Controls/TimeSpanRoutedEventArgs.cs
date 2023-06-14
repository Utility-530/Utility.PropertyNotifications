using System;
using System.Windows;

namespace Utility.FileSystem.Transfer.WPF.Controls
{
    public class TimeSpanRoutedEventArgs : RoutedEventArgs
    {
        public TimeSpanRoutedEventArgs(RoutedEvent routedEvent, TimeSpan timeSpan)
            : base(routedEvent)
            => this.TimeSpan = timeSpan;

        public TimeSpan TimeSpan { get; set; }
    }
}
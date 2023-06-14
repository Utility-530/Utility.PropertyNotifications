using System;
using System.Linq;
using System.Reactive.Linq;
using System.Windows.Controls;

namespace Utility.WPF.Reactive
{
    public static class ScrollViewerHelper
    {
        public static IObservable<ScrollChangedEventArgs> ScrollChanges(this ScrollViewer combo) =>
         Observable
            .FromEventPattern<ScrollChangedEventHandler, ScrollChangedEventArgs>
            (a => combo.ScrollChanged += a, a => combo.ScrollChanged -= a)
            .Select(a => a.EventArgs);
    }
}
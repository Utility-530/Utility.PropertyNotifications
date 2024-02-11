using System;
using System.Linq;
using System.Reactive.Linq;
using System.Windows.Controls;

namespace Utility.WPF.Reactives
{
    public static class ScrollViewerHelper
    {
        public static IObservable<ScrollChangedEventArgs> Changes(this ScrollViewer combo) =>
         Observable
            .FromEventPattern<ScrollChangedEventHandler, ScrollChangedEventArgs>
            (a => combo.ScrollChanged += a, a => combo.ScrollChanged -= a)
            .Select(a => a.EventArgs);
    }
}
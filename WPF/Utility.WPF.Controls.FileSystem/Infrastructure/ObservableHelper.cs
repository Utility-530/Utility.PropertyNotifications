using System;
using System.Linq;
using System.Reactive.Linq;
using System.Windows.Controls;

namespace Utility.WPF.Controls.FileSystem.Infrastructure
{
    public static class ObservableHelper
    {
        public static IObservable<string> SelectPathChanges<T>(this PathBrowser<T> pathBrowser) where T : Control
        {
            return Observable.FromEventPattern<TextChangedRoutedEventHandler, TextChangedRoutedEventArgs>(
                   a => pathBrowser.TextChange += a,
                   a => pathBrowser.TextChange -= a)
                .Select(a => a.EventArgs.Text);
        }
    }
}
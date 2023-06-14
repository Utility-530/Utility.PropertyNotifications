using System;
using System.Linq;
using System.Reactive.Linq;
using System.Windows.Controls;

namespace Utility.WPF.Reactive
{
    public static class TextBoxHelper
    {
        public static IObservable<string> ToThrottledObservable(this TextBox textBox)
        {
            return Observable.FromEventPattern<TextChangedEventHandler, TextChangedEventArgs>(
                   s => textBox.TextChanged += s,
                   s => textBox.TextChanged -= s)
                .Select(evt => textBox.Text) // better to select on the UI thread
                .Throttle(TimeSpan.FromMilliseconds(500))
                .DistinctUntilChanged();
        }
    }
}
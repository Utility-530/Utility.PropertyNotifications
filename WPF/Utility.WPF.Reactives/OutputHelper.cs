using System;
using System.Linq;
using System.Reactive.Linq;
using System.Windows;
using Utility.WPF.Abstract;

namespace Utility.WPF.Reactives
{
    public static class OutputHelper
    {
        public static IObservable<T> SelectOutputChanges<T>(this IOutput<T> selector) where T : RoutedEventArgs =>
            Observable
            .FromEventPattern<OutputChangedEventHandler<T>, T>
            (a => selector.OutputChange += a, a => selector.OutputChange -= a)
            .Select(a => a.EventArgs);
    }
}
using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows;

namespace Utility.WPF.Reactives
{
    public static class FrameworkElementHelper
    {
        public static IObservable<Unit> LoadedChanges(this FrameworkElement element)
        {
            if (element.IsLoaded)
            {
                return Observable.Return(default(Unit));
            }

            var obs = Observable
            .FromEventPattern<RoutedEventHandler, RoutedEventArgs>
            (a => element.Loaded += a, a => element.Loaded -= a)
            .Select(a => Unit.Default);

            return obs;
        }

        public static IDisposable OnLoaded<T>(this T element, Action<T> action) where T : FrameworkElement
        {
            if (element.IsLoaded)
            {
                action(element);
                return Disposable.Empty;
            }

            return Observable
            .FromEventPattern<RoutedEventHandler, RoutedEventArgs>
            (a => element.Loaded += a, a => element.Loaded -= a)
            .Subscribe(a => action(element));
        }

        public static IObservable<RoutedEventArgs> VisibleChanges(this FrameworkElement combo) =>
            Observable
            .FromEventPattern<DependencyPropertyChangedEventHandler, RoutedEventArgs>
            (a => combo.IsVisibleChanged += a, a => combo.IsVisibleChanged -= a)
            .Select(a => a.EventArgs);
    }
}
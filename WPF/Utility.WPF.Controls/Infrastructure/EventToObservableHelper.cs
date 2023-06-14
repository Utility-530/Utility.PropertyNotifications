using System;
using System.Reactive.Linq;
using System.Windows;
using static Utility.WPF.Controls.DateTimeControl;

namespace Utility.WPF.Controls.Infrastructure
{
    public static class EventToObservableHelper
    {
        public static IObservable<DateRangeChangeRoutedEventArgs> DateTimeRangeChanges(this DateTimeControl combo) =>
   Observable
.FromEventPattern<DateTimeRangeChangedEventHandler, DateRangeChangeRoutedEventArgs>
(a => combo.DateRangeChange += a, a => combo.DateRangeChange -= a)
.Select(a => a.EventArgs);

        public static IObservable<decimal> ValueChanges(this SpinnerControl combo) =>
   Observable
.FromEventPattern<RoutedPropertyChangedEventHandler<decimal>, RoutedPropertyChangedEventArgs<decimal>>
(a => combo.ValueChanged += a, a => combo.ValueChanged -= a)
.Select(a => a.EventArgs.NewValue);
    }
}
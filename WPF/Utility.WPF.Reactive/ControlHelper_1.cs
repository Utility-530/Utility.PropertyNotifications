using System;
using System.Linq;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media.Animation;

namespace Utility.WPF.Reactive
{
    public static partial class ControlHelper
    {
        public static IObservable<RoutedEventArgs> Clicks(this Button selector) => from x in Observable.FromEventPattern<RoutedEventHandler, RoutedEventArgs>(a => selector.Click += a, a => selector.Click -= a)
                                                                                     select x.EventArgs;

        public static IObservable<bool?> Changes(this ToggleButton toggleButton)
        {
            return (from a in (from a in Checks()
                               select a).Merge(from a in Unchecks()
                                               select a)
                    select a.IsChecked)
                    .StartWith(toggleButton.IsChecked)
                    .DistinctUntilChanged();

            IObservable<ToggleButton> Checks() => from es in Observable.FromEventPattern<RoutedEventHandler, RoutedEventArgs>(a => toggleButton.Checked += a, a => toggleButton.Checked -= a)
                                                         select es.Sender as ToggleButton;

            IObservable<ToggleButton> Unchecks() => from es in Observable.FromEventPattern<RoutedEventHandler, RoutedEventArgs>(a => toggleButton.Unchecked += a, a => toggleButton.Unchecked -= a)
                                                           select es.Sender as ToggleButton;
        }

        public static IObservable<(double h, double v)> Deltas(this Thumb thumb) => from es in Observable.FromEventPattern<DragDeltaEventHandler, DragDeltaEventArgs>(a => thumb.DragDelta += a, a => thumb.DragDelta -= a)
                                                                                      select (es.EventArgs.HorizontalChange, es.EventArgs.VerticalChange);



        public static IObservable<bool> Toggles(this ToggleButton toggleButton, bool defaultValue = false)
        {
            return toggleButton.Events()
                .Checked.Select(a => true).Merge(toggleButton.Events()
                .Unchecked.Select(a => false))
                .StartWith(toggleButton.IsChecked ?? defaultValue);
        }


        //public static IObservable<ClickRoutedEventArgs<object>> SelectClicks(this CollectionView buttonsItemsControl)
        //{
        //    return Observable.FromEventPattern<ClickRoutedEventHandler<object>, ClickRoutedEventArgs<object>>(
        //           a => buttonsItemsControl.ButtonClick += a,
        //           a => buttonsItemsControl.ButtonClick -= a)
        //        .Select(a => a.EventArgs);
        //}

        public static IObservable<bool> ThrottleToggles(this ToggleButton toggleButton, int milliSeconds = 500)
        {
            return Observable.FromEventPattern<RoutedEventHandler, RoutedEventArgs>(
                    s => toggleButton.Checked += s,
                    s => toggleButton.Checked -= s)
                .Select(evt => true)
                .Merge(Observable.FromEventPattern<RoutedEventHandler, RoutedEventArgs>(
                        s => toggleButton.Unchecked += s,
                        s => toggleButton.Unchecked -= s)
                    .Select(evt => false))
                // better to select on the UI thread
                .Throttle(TimeSpan.FromMilliseconds(milliSeconds))
                .StartWith(toggleButton.IsChecked ?? false)
                .DistinctUntilChanged();
        }


    }
}
using System;
using System.Linq;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media.Animation;

namespace Utility.WPF.Reactive
{
    public static class ButtonHelper
    {
        public static IObservable<RoutedEventArgs> ToClicks(this Button selector) => from x in Observable.FromEventPattern<RoutedEventHandler, RoutedEventArgs>(a => selector.Click += a, a => selector.Click -= a)
                                                                                     select x.EventArgs;

        public static IObservable<bool?> ToChanges(this ToggleButton toggleButton)
        {
            return (from a in (from a in SelectChecked()
                               select a).Merge(from a in SelectUnchecked()
                                               select a)
                    select a.IsChecked)
                    .StartWith(toggleButton.IsChecked)
                    .DistinctUntilChanged();

            IObservable<ToggleButton> SelectChecked() => from es in Observable.FromEventPattern<RoutedEventHandler, RoutedEventArgs>(a => toggleButton.Checked += a, a => toggleButton.Checked -= a)
                                                         select es.Sender as ToggleButton;

            IObservable<ToggleButton> SelectUnchecked() => from es in Observable.FromEventPattern<RoutedEventHandler, RoutedEventArgs>(a => toggleButton.Unchecked += a, a => toggleButton.Unchecked -= a)
                                                           select es.Sender as ToggleButton;
        }

        public static IObservable<(double h, double v)> ToDeltas(this Thumb thumb) => from es in Observable.FromEventPattern<DragDeltaEventHandler, DragDeltaEventArgs>(a => thumb.DragDelta += a, a => thumb.DragDelta -= a)
                                                                                      select (es.EventArgs.HorizontalChange, es.EventArgs.VerticalChange);



        public static IObservable<bool> SelectToggleChanges(this ToggleButton toggleButton, bool defaultValue = false)
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

        public static IObservable<bool> ToThrottledObservable(this ToggleButton toggleButton)
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
                .Throttle(TimeSpan.FromMilliseconds(500))
                .StartWith(toggleButton.IsChecked ?? false)
                .DistinctUntilChanged();
        }


    }
}
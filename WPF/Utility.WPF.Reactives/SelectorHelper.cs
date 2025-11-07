using System;
using System.Collections;
using System.Linq;
using System.Reactive.Linq;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using deniszykov.TypeConversion;
using ReactiveUI;
using Utility.WPF.Abstract;

namespace Utility.WPF.Reactives
{
    public static class SelectorHelper
    {
        public static IObservable<object> Changes(this Selector selector, bool includeNulls = false) =>
            from change in (from a in Observable.FromEventPattern<SelectionChangedEventHandler, SelectionChangedEventArgs>(a => selector.SelectionChanged += a, a => selector.SelectionChanged -= a)
                            select a.EventArgs.AddedItems.Cast<object>().SingleOrDefault())
            .StartWith(selector.SelectedItem)
            where change is not null || includeNulls
            select change;

        public static IObservable<object> Changes(this ISelector selector, bool includeNulls = false) =>
            from change in (from a in Observable.FromEventPattern<SelectionChangedEventHandler, SelectionChangedEventArgs>(a => selector.SelectionChanged += a, a => selector.SelectionChanged -= a)
                            select a.EventArgs.AddedItems.Cast<object>().SingleOrDefault())
            .StartWith(selector.SelectedItem)
            where change is not null || includeNulls
            select change;

        public static IObservable<T> Changes<T>(this Selector selector, bool includeNulls = false) => selector.Changes(includeNulls).Cast<T>();

        public static IObservable<T> Changes<T>(this ISelector selector, bool includeNulls = false) => selector.Changes(includeNulls).Cast<T>();

        public static IObservable<ListBoxItem[]> MultiAdditions(this Selector selector) =>
            Observable
            .FromEventPattern<SelectionChangedEventHandler, SelectionChangedEventArgs>
            (a => selector.SelectionChanged += a, a => selector.SelectionChanged -= a)
            .Select(a => selector.Items
                                                            .Cast<ListBoxItem>()
                                                            .Where(a => a.IsSelected)
                                                            .ToArray());

        public static IObservable<IList> Additions(this Selector selector) =>

            Observable
            .FromEventPattern<SelectionChangedEventHandler, SelectionChangedEventArgs>
            (a => selector.SelectionChanged += a, a => selector.SelectionChanged -= a)
            .Select(a => a.EventArgs.AddedItems)
            .StartWith(selector.SelectedItem != null ? new[] { selector.SelectedItem } : Array.Empty<object>() as IList)
            .Where(a => a.Count > 0);

        public static IObservable<IList> Additions(this ISelector selector) =>

            Observable
            .FromEventPattern<SelectionChangedEventHandler, SelectionChangedEventArgs>
            (a => selector.SelectionChanged += a, a => selector.SelectionChanged -= a)
            .Select(a => a.EventArgs.AddedItems)
            .StartWith(selector.SelectedItem != null ? new[] { selector.SelectedItem } : Array.Empty<object>() as IList)
            .Where(a => a.Count > 0);

        // public static IObservable<object> SelectMultiSelectionChanges(this ISelector selector) =>
        //Observable
        //.FromEventPattern<SelectionChangedEventHandler, SelectionChangedEventArgs>
        //(a => selector.SelectionChanged += a, a => selector.SelectionChanged -= a)
        //.Select(a => a.EventArgs.AddedItems)
        //.Select(a => a.Cast<object>().Single());

        public static IObservable<IList> Removals(this Selector selector) =>
            Observable
            .FromEventPattern<SelectionChangedEventHandler, SelectionChangedEventArgs>
            (a => selector.SelectionChanged += a, a => selector.SelectionChanged -= a)
            .Select(a => a.EventArgs.RemovedItems)
            .Where(a => a.Count > 0);

        public static IObservable<object> DistinctAdditions(this Selector selector) =>
            Observable
            .FromEventPattern<SelectionChangedEventHandler, SelectionChangedEventArgs>
            (a => selector.SelectionChanged += a, a => selector.SelectionChanged -= a)
            .Select(a => a.EventArgs.AddedItems)
            .StartWith(selector.SelectedItem != null ? new[] { selector.SelectedItem } : Array.Empty<object>() as IList)
            .Where(a => a.Count == 1)
            .DistinctUntilChanged()
            .Select(a => a.Cast<object>().Single());

        public static IObservable<object> DistinctAdditions(this ISelector selector) =>
            Observable
            .FromEventPattern<SelectionChangedEventHandler, SelectionChangedEventArgs>
            (a => selector.SelectionChanged += a, a => selector.SelectionChanged -= a)
            .Select(a => a.EventArgs.AddedItems)
            .StartWith(selector.SelectedItem != null ? new[] { selector.SelectedItem } : Array.Empty<object>() as IList)
            .Where(a => a.Count == 1)
            .Select(a => a.Cast<object>().Single());

        public static IObservable<object> ValueChanges(this Selector selector) =>

            Observable
                .FromEventPattern<SelectionChangedEventHandler, SelectionChangedEventArgs>
                    (a => selector.SelectionChanged += a, a => selector.SelectionChanged -= a)
                .Select(a => selector.SelectedValue).StartWith(selector.SelectedValue)
                .WhereNotNull();

        public static IObservable<T> ToSelectedValueChanges<T>(this Selector selector) =>
            selector.ValueChanges().Cast<T>();

        public static IObservable<T?> ItemChanges<T>(this Selector selector)
        {
            var selectionChanged = selector.Events().SelectionChanged;
            var conversionProvider = new TypeConversionProvider();
            // If using ComboBoxItems
            var comboBoxItems = selectionChanged
          .SelectMany(a => a.AddedItems.OfType<ContentControl>())
          .StartWith(selector.SelectedItem as ContentControl)
          .Where(a => a != null)
          .Select(a => NewMethod2(a?.Content))
            .Where(a => a?.Equals(default(T)) == false);

            // If using type directly
            var directItems = selectionChanged
          .SelectMany(a => a.AddedItems.OfType<T>())
          .StartWith(NewMethod(selector.SelectedItem))
          .Where(a => a?.Equals(default(T)) == false);

            // If using type indirectly
            var indirectItems = selectionChanged
          .SelectMany(a => a.AddedItems.Cast<object>().Select(a => conversionProvider.TryConvert(a, out T t2) ? t2 : default))
          .StartWith(NewMethod2(selector.SelectedItem))
          .Where(a => a?.Equals(default(T)) == false);

            var c = comboBoxItems.Amb(directItems).Amb(indirectItems);

            return c;

            static T? NewMethod(object? selectedItem)
            {
                return selectedItem is T t ? t : default;
            }

            T? NewMethod2(object? selectedItem)
            {
                return conversionProvider.TryConvert(selectedItem, out T t2) ? t2 : default;
            }
        }
    }
}
using deniszykov.TypeConversion;
using ReactiveUI;
using System;
using System.Collections;
using System.Linq;
using System.Reactive.Linq;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Utility.WPF.Abstract;

namespace Utility.WPF.Reactive
{
    public static class SelectorHelper
    {

        public static IObservable<object> ToChanges(this Selector selector) =>
            from change in (from a in Observable.FromEventPattern<SelectionChangedEventHandler, SelectionChangedEventArgs>(a => selector.SelectionChanged += a, a => selector.SelectionChanged -= a)
                            select a.EventArgs.AddedItems.Cast<object>().SingleOrDefault())
            .StartWith(selector.SelectedItem)
            where change != null
            select change;

        public static IObservable<ListBoxItem[]> SelectMultiSelectionChanges(this Selector selector) =>
            Observable
            .FromEventPattern<SelectionChangedEventHandler, SelectionChangedEventArgs>
            (a => selector.SelectionChanged += a, a => selector.SelectionChanged -= a)
            .Select(a => selector.Items
                                                            .Cast<ListBoxItem>()
                                                            .Where(a => a.IsSelected)
                                                            .ToArray());
        public static IObservable<IList> SelectSelectionAddChanges(this Selector selector) =>

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

        public static IObservable<IList> SelectSelectionRemoveChanges(this Selector selector) =>
            Observable
            .FromEventPattern<SelectionChangedEventHandler, SelectionChangedEventArgs>
            (a => selector.SelectionChanged += a, a => selector.SelectionChanged -= a)
            .Select(a => a.EventArgs.RemovedItems)
            .Where(a => a.Count > 0);

        public static IObservable<object> SelectSingleSelectionChanges(this Selector selector) =>
            Observable
            .FromEventPattern<SelectionChangedEventHandler, SelectionChangedEventArgs>
            (a => selector.SelectionChanged += a, a => selector.SelectionChanged -= a)
            .Select(a => a.EventArgs.AddedItems)
            .StartWith(selector.SelectedItem != null ? new[] { selector.SelectedItem } : Array.Empty<object>() as IList)
            .Where(a => a.Count == 1)
            .DistinctUntilChanged()
            .Select(a => a.Cast<object>().Single());

        public static IObservable<object> SelectSingleSelectionChanges(this ISelector selector) =>
            Observable
            .FromEventPattern<SelectionChangedEventHandler, SelectionChangedEventArgs>
            (a => selector.SelectionChanged += a, a => selector.SelectionChanged -= a)
            .Select(a => a.EventArgs.AddedItems)
            .StartWith(selector.SelectedItem != null ? new[] { selector.SelectedItem } : Array.Empty<object>() as IList)
            .Where(a => a.Count == 1)
            .Select(a => a.Cast<object>().Single());

        public static IObservable<object> ToSelectedValueChanges(this Selector selector) =>

            Observable
                .FromEventPattern<SelectionChangedEventHandler, SelectionChangedEventArgs>
                    (a => selector.SelectionChanged += a, a => selector.SelectionChanged -= a)
                .Select(a => selector.SelectedValue).StartWith(selector.SelectedValue)
                .WhereNotNull();
        public static IObservable<T> ToSelectedValueChanges<T>(this Selector selector) =>
            selector.ToSelectedValueChanges().Cast<T>();


        public static IObservable<T?> SelectItemChanges<T>(this Selector selector)
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
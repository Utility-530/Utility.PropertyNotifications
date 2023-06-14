using DynamicData;
using ReactiveUI;
using System;
using System.Linq;
using System.Reactive.Linq;
using System.Windows.Controls;
using Utility.Reactive;

namespace Utility.WPF.Reactive
{
    public static class ItemsControlHelper
    {
        public static IObservable<int> CountChanges(this ItemsControl headeredItemsControl)
        {
            return Utility.Reactive.ObservableHelper.Pace(headeredItemsControl.WhenAnyValue(a => a.ItemsSource)
                .WhereNotNull()
                .Select(a =>Utility.Helpers.Ex.EnumerableHelper.ToGenericObservable(a).ToObservableChangeSet()
                .ToCollection()
                .Select(a => a.Count)), TimeSpan.FromSeconds(0.3))
                .DistinctUntilChanged()
                .Switch();
        }
    }
}
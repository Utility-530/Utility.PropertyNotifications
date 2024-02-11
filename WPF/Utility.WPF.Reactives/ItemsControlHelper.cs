using DynamicData;
using ReactiveUI;
using System;
using System.Linq;
using System.Reactive.Linq;
using System.Windows.Controls;
using Utility.Reactives;

namespace Utility.WPF.Reactives
{
    public static class ItemsControlHelper
    {
        public static IObservable<int> Counts(this ItemsControl headeredItemsControl)
        {
            return TimeHelper.Pace(headeredItemsControl.WhenAnyValue(a => a.ItemsSource)
                .WhereNotNull()
                .Select(a =>Utility.Helpers.Ex.EnumerableHelper.ToGenericObservable(a).ToObservableChangeSet()
                .ToCollection()
                .Select(a => a.Count)), TimeSpan.FromSeconds(0.3))
                .DistinctUntilChanged()
                .Switch();
        }
    }
}
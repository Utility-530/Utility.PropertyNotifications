using System;
using System.Linq;
using System.Reactive.Linq;
using System.Windows.Controls;
using Utility.Helpers.Ex;
using Utility.Reactives;

namespace Utility.WPF.Reactives
{
    public static class ItemsControlHelper
    {
        public static IObservable<int> Counts(this ItemsControl headeredItemsControl)
        {
            return TimeHelper.Pace(headeredItemsControl.Observe(HeaderedItemsControl.ItemsSourceProperty)
                .Select(a => headeredItemsControl.ItemsSource)
                .Where(a => a != null)
                .Select(a => Utility.Helpers.Ex.EnumerableHelper.ToGenericObservable(a).ToObservableCollection())
                .Select(a => a.Count), TimeSpan.FromSeconds(0.3))
                .DistinctUntilChanged();
              
        }
    }
}
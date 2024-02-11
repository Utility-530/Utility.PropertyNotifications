using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using Utility.WPF.Abstract;
using Utility.WPF.Events;

namespace Utility.WPF.Reactives
{
    public static class CheckedSelectorHelper
    {

        public static IObservable<CheckedChangedEventArgs> SelectCheckedChangedEventArgs(this ICheckedSelector selector) =>
            Observable
            .FromEventPattern<CheckedChangedEventHandler, CheckedChangedEventArgs>
            (a => selector.CheckedChanged += a, a => selector.CheckedChanged -= a)
            .Select(a => a.EventArgs);

        public static IObservable<IReadOnlyCollection<(object obj, bool isChecked)>> SelectCheckedAndUnCheckedItems(this ICheckedSelector selector) =>
    Observable
    .FromEventPattern<CheckedChangedEventHandler, CheckedChangedEventArgs>
    (a => selector.CheckedChanged += a, a => selector.CheckedChanged -= a)
    .Select(a => a.EventArgs.Checked.Select(a => (a, true)).Concat(a.EventArgs.UnChecked.Select(a => (a, false))).ToArray());

        public static IObservable<IReadOnlyCollection<object>> SelectCheckedItems(this ICheckedSelector selector) =>
            SelectCheckedChangedEventArgs(selector).Select(a => a.Checked);

        public static IObservable<IReadOnlyCollection<object>> SelectUnCheckedItems(this ICheckedSelector selector) =>
                    SelectCheckedChangedEventArgs(selector).Select(a => a.UnChecked);
    }
}

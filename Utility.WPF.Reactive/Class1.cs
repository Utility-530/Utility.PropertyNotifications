using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Windows;
using Utility.WPF.Helper;
using Utility.Helpers;
using Utility.Reactive;

namespace Utility.WPF.Reactive;

public static class ChildControls
{
    static Dictionary<FrameworkElement, IObservable<FrameworkElement>> Lazy { get; } = new();

    public static IObservable<T> Observable<T>(this FrameworkElement dependencyObject, string? name = null) where T : FrameworkElement
    {
        return System.Reactive.Linq.Observable.Create<T>(observer =>
        {
            var lazy = Lazy.GetValueOrNew(dependencyObject,ControlsOnLoad(dependencyObject).SelectMany());
            var dis = (name == null ?
             lazy.OfType<T>() :
             lazy.OfType<T>().Where(a => a.Name == name))
             .Subscribe(a =>
             {
                 observer.OnNext(a);
             });
            //observer.OnCompleted();
            return dis;
        })
        .ObserveOnDispatcher()
        .SubscribeOnDispatcher();
    }

    static IObservable<IEnumerable<FrameworkElement>> ControlsOnLoad(this FrameworkElement dependencyObject)
    {
        if (dependencyObject is FrameworkElement control)
        {
            if (control.IsLoaded == false)
            {
                return System.Reactive.Linq.Observable.Create<FrameworkElement[]>(observer =>
                {
                    return control
                        .LoadedChanges()
                        .Subscribe(a =>
                        {
                            var t = dependencyObject
                            .FindVisualChildren<FrameworkElement>()
                            .ToArray();
                            observer.OnNext(t);
                        });
                })
                     .ObserveOnDispatcher()
                     .SubscribeOnDispatcher()
                     .ToReplaySubject(1);
            }
        }
        return System.Reactive.Linq
            .Observable
            .Return(dependencyObject
            .FindVisualChildren<FrameworkElement>()
            .ToArray());
    }
}

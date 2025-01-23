using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Utility.Changes;

namespace Utility.Nodes.Common
{
    public static class ChangeHelper
    {
        public static IObservable<Change> Filter<T>(this IObservable<T> observable, IObservable<bool> filter)
        {
            return Observable.Create<Change>(observer =>
            {
                List<object> list = new();
                CompositeDisposable c = new();
                observable.Subscribe(a =>
                {
                    if (a is Change change)
                    {
                        if (change.Type == Changes.Type.Add)
                        {
                            list.Add(change.Value);
                        }
                        else if (change.Type == Changes.Type.Remove)
                        {
                            if (list.Contains(change.Value))
                                list.Remove(change.Value);
                        }
                    }
                    else
                        list.Add(a);

                }).DisposeWith(c);

                filter.Subscribe(a =>
                {
                    foreach (var x in list)
                    {
                        observer.OnNext(new Change(x, null, a ? Changes.Type.Add : Changes.Type.Remove));
                    }

                }).DisposeWith(c);
                return c;
            });
        }
    }
}

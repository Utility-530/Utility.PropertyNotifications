using System.Collections.ObjectModel;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Utility.Changes;
using Utility.Observables.Generic;
using Utility.Reactives;

namespace Utility.Nodes.Common
{
    public static class ChangeHelper
    {
        private static bool isInitialised;

        public static IObservable<Change> Filter<T>(this IObservable<T> observable, IObservable<bool> filter)
        {
            return Observable.Create<Change>(observer =>
            {
                ObservableCollection<object> collection = [];
                CompositeDisposable composite = [];
                IDisposable? dis = null;

                observable.Subscribe(a =>
                {
                    if (a is Change change)
                    {
                        if (change.Type == Changes.Type.Add)
                        {
                            collection.Add(change.Value);
                        }
                        else if (change.Type == Changes.Type.Remove)
                        {
                            if (collection.Contains(change.Value))
                                collection.Remove(change.Value);
                        }
                    }
                    else
                        collection.Add(a);
                }).DisposeWith(composite);

                filter
                .Subscribe(_filter =>
                {
                    if (_filter == false && isInitialised == false)
                    {
                        return;
                    }
                    isInitialised = true;
                    foreach (var x in collection)
                    {
                        observer.OnNext(new Change(x, null, _filter ? Changes.Type.Add : Changes.Type.Remove));
                    }
                    if (_filter)
                    {
                        dis?.Dispose();
                        dis = collection
                                .Changes()
                                .Subscribe(a =>
                                {
                                    observer.OnNext(a);
                                }).DisposeWith(composite);
                    }
                }).DisposeWith(composite);

                return composite;
            });
        }
    }
}
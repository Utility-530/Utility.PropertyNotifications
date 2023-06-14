using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility.Helpers.Ex
{
    public static class ObservableHelper
    {

        public static IObservable<T> WhereNotDefault<T>(this IObservable<T> observable)
        {
            return observable.Where(a => !(a?.Equals(default(T)) ?? false));
        }

        public static IObservable<T> ToObservable<TCollection, T>(this TCollection oc)
    where TCollection : IEnumerable<T> =>
    Observable.Create<T>(observer =>
    {
        foreach (var obj in oc)
            observer.OnNext(obj);

        return oc is INotifyCollectionChanged notifyCollectionChanged
        ? notifyCollectionChanged
            .SelectNewItems<T>()
            .Subscribe(observer.OnNext)
        : Disposable.Empty;
    });


    }

    public static class EnumerableHelper
    {
        public static IObservable<object> ToGenericObservable(this IEnumerable oc) =>
Observable.Create<object>(observer =>
{
    foreach (var obj in oc.Cast<object>())
        observer.OnNext(obj);

    return oc is INotifyCollectionChanged notifyCollectionChanged
        ? notifyCollectionChanged
            .SelectNewItems<object>()
            .Subscribe(observer.OnNext)
        : Disposable.Empty;
});
    }


}



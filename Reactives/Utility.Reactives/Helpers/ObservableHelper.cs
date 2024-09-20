using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;

namespace Utility.Reactives;

/// <summary>
/// ObservableCollection  helper
/// </summary>
public static partial class ObservableHelper
{

    public static IObservable<T[]> Combine<T>(this IEnumerable<IObservable<T>> observables)
    {
        return Observable.Create<T[]>(observer =>
        {
            CompositeDisposable disposables = new();
            var length = observables.ToArray().Length;
            int count = 0;

            Dictionary<IObservable<T>, T> keyValuePairs = new();

            foreach (var obs in observables)
            {
                obs.Subscribe(a =>
                {
                    count++;
                    keyValuePairs[obs] = a;

                    if (length == count)
                    {
                        observer.OnNext(keyValuePairs.Select(a => a.Value).ToArray());
                    }
                }, () =>
                {
                  
                }).DisposeWith(disposables);
            }
            return disposables;
        });
        // return observable.SelectMany().Subscribe(observer);
    }


    public static IDisposable SubscribeMany<T>(this IEnumerable<IObservable<T>> observable, IObserver<T> observer)
    {
        var cd = new CompositeDisposable();
        foreach (var em in observable)
        {
            cd.Add(em.Subscribe(observer));
        }
        return cd;

        // return observable.SelectMany().Subscribe(observer);
    }

    public static IDisposable SubscribeMany<T>(this IObservable<IEnumerable<T>> observable, IObserver<T> observer)
    {
        return observable.SelectMany().Subscribe(observer);
    }

    //public static IObservable<T> SelectMany<T>(this IEnumerable<IObservable<T>> observable)
    //{
    //    return Observable.ToObservable(observable).SelectMany(a => a);
    //}

    public static IObservable<T> SelectMany<T>(this IObservable<IEnumerable<T>> observable)
    {
        return observable.SelectMany(a => a);
    }

    public static IObservable<T> SelectMany<T>(this IObservable<IObservable<T>> observable)
    {
        return observable.SelectMany(a => a);
    }

    //public static IObservable<T> WhereNotNull<T>(this IObservable<T> observable) where T : class
    //{
    //    return observable.Where(a => a != null);
    //}

    public static IObservable<IEnumerable<T>> WhereAny<T>(this IObservable<IEnumerable<T>> observable)
    {
        return observable.Where(a => a.Any());
    }

    public static IObservable<T> TakeLastItem<T>(this IObservable<IEnumerable<T>> observable)
    {
        return observable.Select(a => a.Last());
    }

    public static IObservable<T> Select<T>(this Task<IEnumerable<T>> enumerableTask)
    {
        return enumerableTask.ToObservable().SelectMany();
    }

    public static IObservable<T> Select<T>(this Task<IObservable<T>> enumerableTask)
    {
        return enumerableTask.ToObservable().SelectMany();
    }

    /// <summary>
    /// </summary>
    /// <param name="oc"></param>
    /// <returns>the property-names associated with the error events</returns>
    public static IObservable<string?> SelectErrors(this INotifyDataErrorInfo oc)
    {
        return Observable
            .FromEventPattern<EventHandler<DataErrorsChangedEventArgs>, DataErrorsChangedEventArgs>(
                h => oc.ErrorsChanged += h,
                h => oc.ErrorsChanged -= h)
            .Select(a => a.EventArgs.PropertyName);
    }

    //public static IObservable<T> SelectAddedItems<T>(this INotifyCollectionChanged notifyCollectionChanged)
    //{
    //    return notifyCollectionChanged
    //        .SelectChanges()
    //        .SelectMany(x => x.NewItems?.Cast<T>() ?? new T[] { })
    //        .WhereNotDefault();
    //}

    public static IObservable<(T, bool predicate)> ScanChanges<T, TR>(this IObservable<T> source, Func<T?, TR> property) where T : struct where TR : IEquatable<TR>
    {
        return source
            .Scan((default(T), false), (a, b) => (b, property(a.Item1).Equals(property(b)) == false))
            .Skip(1)!;
    }

    public static IObservable<(T, bool predicate)> ScanRefChanges<T, TR>(this IObservable<T> source, Func<T, TR> property) where T : class where TR : IEquatable<TR>
    {
        return source
            .Scan((default(T), false), (a, b) => (b, a.Item1 != null && property(a.Item1).Equals(property(b)) == false))
            .Skip(1)!;
    }

    public static IObservable<(T value, int index)> Index<T>(this IObservable<T> source)
    {
        return source
            .Scan((default(T), -1), (a, b) => (b, ++a.Item2))
            .Skip(1)!;
    }

    public static IObservable<TR> Choose<T, TR>(this IObservable<T> source, Func<T, bool> predicate, Func<T, TR> func)
    {
        return source
            .Where(a => predicate(a))
            .Select(func);
    }

    public static ReplaySubject<T> ToReplaySubject<T>(this IObservable<T> source, int save = 1)
    {
        var replaySubject = new ReplaySubject<T>(save);
        source.Subscribe(replaySubject);
        return replaySubject;
    }

    //public static IObservable<T> SelectMany<T>(this IObservable<IEnumerable<T>> manyObservable)
    //{
    //    return manyObservable.SelectMany(a => a);
    //}

    /// <summary>
    /// <a href="https://stackoverflow.com/questions/28853030/iobservable-ignore-new-elements-for-a-span-of-time"></a>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="sampleDuration"></param>
    /// <param name="scheduler"></param>
    /// <returns></returns>
    public static IObservable<T> SampleFirst<T>(
this IObservable<T> source,
TimeSpan sampleDuration,
IScheduler? scheduler = null)
    {
        scheduler ??= Scheduler.Default;
        return source.Publish(ps =>
            ps.Window(() => ps.Delay(sampleDuration, scheduler))
              .SelectMany(x => x.Take(1)));
    }

    public static IObservable<(T, R)> Join<T, R>(this IObservable<T> observable, IObservable<R> observable2, Func<T, R, bool> join)
    {
        return observable.SelectMany(a => observable2.Select(b => (a, b)).Where(c => join(c.a, c.b)));
    }

    public static IObservable<S> JoinRightUntil<T, R, S>(this IObservable<T> observable, IObservable<R> observable2,
        Func<T, R, bool> until, Func<T, R, S> selector
        )
    {
        return Observable.Create<S>(observer =>
        {
            var date = DateTime.Now;
            return observable2.Subscribe(b =>
            {
                bool dispose = false;
                IDisposable? disposable = default;
                disposable = observable
                .Subscribe(a =>
                {
                    if (until(a, b) && dispose == false)
                    {
                        if (disposable == default)
                            //throw new NullReferenceException("Disposable is null. dsffsd");
                            dispose = true;
                        else
                            disposable.Dispose();

                        observer.OnNext(selector(a, b));
                    }
                });
                if (dispose == true)
                    disposable.Dispose();
            });
        });
    }

    public static IObservable<S> JoinLeftUntil<T, R, S>(
        this IObservable<T> observable,
        IObservable<R> observable2,
        Func<T, R, bool> until,
        Func<T, R, S> selector)
    {
        return Observable.Create<S>(observer =>
        {
            return observable.Subscribe(b =>
            {
                bool dispose = false;
                IDisposable? disposable = default;
                disposable = observable2.Subscribe(a =>
                {
                    if (dispose)
                        if (disposable == default)
                            //throw new NullReferenceException("Disposable is null. dsffsd");
                            dispose = true;
                        else
                            disposable.Dispose();

                    if (until(b, a) && dispose == false)
                    {
                        dispose = true;
                        disposable?.Dispose();
                        observer.OnNext(selector(b, a));
                    }
                });
                if (dispose == true)
                    disposable.Dispose();
            });
        });
    }

    /// <summary>
    /// Ensures late subscribers from <see cref="observableLeft"/>
    /// get notified of latest change in <see cref="observableRight"/>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="R"></typeparam>
    /// <typeparam name="S"></typeparam>
    /// <param name="observableLeft"></param>
    /// <param name="observableRight"></param>
    /// <param name="selector"></param>
    /// <returns></returns>
    public static IObservable<S> JoinRight<T, R, S>(this IObservable<T> observableLeft, IObservable<R> observableRight, Func<T, R, S> selector)
    {
        return Observable.Create<S>(observer =>
        {
            var date = DateTime.Now;
            IDisposable? disposable = default;
            return observableRight.Subscribe(b =>
            {
                // reset disposable
                disposable?.Dispose();
                disposable = observableLeft
                   .Subscribe(a =>
                   {
                       observer.OnNext(selector(a, b));
                   });
            });
        });
    }

    public static IObservable<T> WhereNotDefault<T>(this IObservable<T> observable)
    {
        return observable.Where(a => !(a?.Equals(default(T)) ?? false));
    }

    public static IObservable<T> WhereIsNotNull<T>(this IObservable<T> observable) where T : class
    {
        return observable.Where(a => a != null);
    }

    public static IObservable<T> BufferTakeLast<T>(this IObservable<T> observable, TimeSpan buffer)
    {
        return observable.Buffer(buffer).WhereAny().TakeLastItem();
    }

    public static IObservable<IEnumerable<T>> WhereEmpty<T>(this IObservable<IEnumerable<T>> observable)
    {
        return observable.Where(a => a.Any() == false);
    }

    public static IObservable<(T, R)> Combine<T, R>(this IObservable<T> observable, R observed)
    {
        return observable
            .Scan((default(T), observed), (a, b) => (b, a.Item2))
            .Skip(1);
    }

    public static IObservable<(T?, TR?)> Combine<T, TR>(this IObservable<T> observable, IObservable<TR> observable2)
        where T : struct
        where TR : struct
    {
        var subject = new ReplaySubject<(T?, TR?)>(1);

        observable.Subscribe(a =>
        {
            subject.OnNext((a, null));
        });

        observable2.Subscribe(b =>
        {
            subject.OnNext((null, b));
        });

        return subject;
    }

    public static IObservable<(T?, TR?)> CombineRefs<T, TR>(this IObservable<T> observable, IObservable<TR> observable2)
        where T : class
        where TR : class
    {
        var subject = new ReplaySubject<(T?, TR?)>(1);

        observable.Subscribe(a =>
        {
            subject.OnNext((a, null));
        });

        observable2.Subscribe(b =>
        {
            subject.OnNext((null, b));
        });

        return subject;
    }

    public static IObservable<T> OfGenericType<T>(this IObservable<object> value, Type? type = null)
    {
        return value.Select(v => Utility.Helpers.TypeHelper.OfType<T>(v, type)).WhereNotDefault();
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Disposables;
using System.Reactive;
using System.Reactive.Subjects;
using System.Reactive.Concurrency;
using System.Collections.Immutable;
using Utility.Reactive;

namespace Utility.Reactive
{

    public static partial class ObservableHelper
    {
        public static IObservable<IEnumerable<T>> Limit<T>(this IObservable<IEnumerable<T>> ss, IObservable<int> bs)
        {
            return ss.CombineLatest(bs, (a, b) => a.Take(b));

        }

        public static IObservable<R> SelectNotNull<T, R>(this IObservable<T> obs, Func<T, R> f)
        {
            return obs.Select(a => f(a)).Where(a => a != null);
        }

        public static IObservable<T> TakeWhile<T>(this IObservable<T> ss, IObservable<bool> bs, bool bl = true)
        {
            return ss.CombineLatest(bs, (a, b) => new { a, b })
                .Where(c => c.b == bl)
                .Select(a => a.a);
        }

        //How to Merge two Observables so the result completes when the any of the Observables completes?
        //Ray Booysen
        public static IObservable<T> MergeWithCompleteOnEither<T>(this IObservable<T> source, IObservable<T> right)
        {
            return Observable.Create<T>(obs =>
            {
                var compositeDisposable = new CompositeDisposable();
                var subject = new Subject<T>();

                compositeDisposable.Add(subject.Subscribe(obs));
                compositeDisposable.Add(source.Subscribe(subject));
                compositeDisposable.Add(right.Subscribe(subject));


                return compositeDisposable;

            });
        }


        //Takes source until right stops emitting values
        public static IObservable<T> TakeUntilEnd<T, R>(this IObservable<T> source, IObservable<R> right)
        {
            return source.TakeUntil(right.GetAwaiter());
        }

        public static IObservable<T> BufferUntilInactive<T>(this IObservable<T> stream, TimeSpan delay)
        {
            var closes = stream.Throttle(delay);
            return stream.Window(() => closes)
                .SelectMany(window => window.ToList())
                .Where(a => a != null && a.Count > 0)
                .SelectMany(a => a)
                .Where(df => df != null);
        }

        public static IObservable<T> BufferUntilInactive<T>(this IObservable<T> t)
        {
            return t.BufferUntilInactive(TimeSpan.FromMilliseconds(300));

        }


        public static IObservable<T> WaitFor<T>(this IObservable<T> source, Func<T, bool> pred)
        {
            return
                source
                    .Where(pred)
                    .DistinctUntilChanged()
                    .Take(1);
        }

        public static IObservable<IObservable<KeyValuePair<int, T>>> RangeToRandomObservable<T>(this IObservable<Savage.Range.Range<int>> output, Func<int, T> func, int size = 10)
        {
            Random r = new Random();
            var obs = Observable.Create<IObservable<KeyValuePair<int, T>>>(observer =>
              output.Where(a => !a.Equals(default(Savage.Range.Range<int>))).Subscribe(a =>
              {
                  var diff = a.Ceiling - a.Floor;

                  observer.OnNext(Observable.Generate(a.Floor,
                   i => i < a.Ceiling, i => i + 1,
                   i =>
                   {
                       return new KeyValuePair<int, T>(i, func(i));
                   },
                   i => TimeSpan.FromSeconds(r.Next(0, size))));

              }));

            return obs;
        }


        // Thursday, August 28, 2014 9:59 AM  Dave Sexton
        //https://social.msdn.microsoft.com/Forums/en-US/cb1f83b0-5fc5-47b3-ad28-465ba4a5d140/how-to-combine-n-observables-dynamically-with-combinelatest-semantics?forum=rx
        // Merges multiple observable sequences
        public static IObservable<IList<TSource>> CombineLatestRef<TSource>(this IEnumerable<IObservable<TSource>> sources)
                  where TSource : class
        {
            return Observable.Create<IList<TSource>>(
                observer =>
                {
                    object gate = new object();
                    var disposables = new CompositeDisposable();
                    var list = new List<TSource>();
                    var hasValueFlags = new List<bool>();
                    var actionSubscriptions = 0;
                    bool hasSources, hasValueFromEach = false;

                    using (var e = sources.GetEnumerator())
                    {
                        bool subscribing = hasSources = e.MoveNext();

                        while (subscribing)
                        {
                            var source = e.Current;
                            int index;

                            lock (gate)
                            {
                                actionSubscriptions++;

                                list.Add(default);
                                hasValueFlags.Add(false);

                                index = list.Count - 1;

                                subscribing = e.MoveNext();
                            }

                            disposables.Add(
                                source.Subscribe(
                                    value =>
                                    {
                                        IList<TSource> snapshot;

                                        lock (gate)
                                        {
                                            list[index] = value;

                                            if (!hasValueFromEach)
                                            {
                                                hasValueFlags[index] = true;

                                                if (!subscribing)
                                                {
                                                    hasValueFromEach = hasValueFlags.All(b => b);
                                                }
                                            }

                                            if (subscribing || !hasValueFromEach)
                                            {
                                                snapshot = null;
                                            }
                                            else
                                            {
                                                snapshot = list.ToList().AsReadOnly();
                                            }
                                        }

                                        if (snapshot != null)
                                        {
                                            observer.OnNext(snapshot);
                                        }
                                    },
                                    observer.OnError,
                                    () =>
                                    {
                                        bool completeNow;

                                        lock (gate)
                                        {
                                            actionSubscriptions--;

                                            completeNow = actionSubscriptions == 0 && !subscribing;
                                        }

                                        if (completeNow)
                                        {
                                            observer.OnCompleted();
                                        }
                                    }));
                        }
                    }

                    if (!hasSources)
                    {
                        observer.OnCompleted();
                    }

                    return disposables;
                });
        }


        // Thursday, August 28, 2014 9:59 AM  Dave Sexton
        //https://social.msdn.microsoft.com/Forums/en-US/cb1f83b0-5fc5-47b3-ad28-465ba4a5d140/how-to-combine-n-observables-dynamically-with-combinelatest-semantics?forum=rx
        // Merges multiple observable sequences
        public static IObservable<IList<TSource?>> CombineLatest<TSource>(this IEnumerable<IObservable<TSource>> sources)
                  where TSource : struct
        {
            return Observable.Create<IList<TSource?>>(
                observer =>
                {
                    object gate = new object();
                    var disposables = new CompositeDisposable();
                    var list = new List<TSource?>();
                    var hasValueFlags = new List<bool>();
                    var actionSubscriptions = 0;
                    bool hasSources, hasValueFromEach = false;

                    using (var e = sources.GetEnumerator())
                    {
                        bool subscribing = hasSources = e.MoveNext();

                        while (subscribing)
                        {
                            var source = e.Current;
                            int index;

                            lock (gate)
                            {
                                actionSubscriptions++;

                                list.Add(default(TSource));
                                hasValueFlags.Add(false);

                                index = list.Count - 1;

                                subscribing = e.MoveNext();
                            }

                            disposables.Add(
                                source.Subscribe(
                                    value =>
                                    {
                                        IList<TSource?> snapshot;

                                        lock (gate)
                                        {
                                            list[index] = value;

                                            if (!hasValueFromEach)
                                            {
                                                hasValueFlags[index] = true;

                                                if (!subscribing)
                                                {
                                                    hasValueFromEach = hasValueFlags.All(b => b);
                                                }
                                            }

                                            if (subscribing || !hasValueFromEach)
                                            {
                                                snapshot = null;
                                            }
                                            else
                                            {
                                                snapshot = list.ToList().AsReadOnly();
                                            }
                                        }

                                        if (snapshot != null)
                                        {
                                            observer.OnNext(snapshot);
                                        }
                                    },
                                    observer.OnError,
                                    () =>
                                    {
                                        bool completeNow;

                                        lock (gate)
                                        {
                                            actionSubscriptions--;

                                            completeNow = actionSubscriptions == 0 && !subscribing;
                                        }

                                        if (completeNow)
                                        {
                                            observer.OnCompleted();
                                        }
                                    }));
                        }
                    }

                    if (!hasSources)
                    {
                        observer.OnCompleted();
                    }

                    return disposables;
                });
        }
        public static IObservable<TResult> Zip<TSource, TResult>(this IEnumerable<IObservable<TSource>> sources, Func<IList<TSource>, TResult> selector)
        {
            return Observable.Defer(() =>
            {
                IObservable<List<TSource>> intermediate = null;

                foreach (var source in sources)
                {
                    if (intermediate == null)
                        intermediate = source.Select(value => new List<TSource>() { value });
                    else
                        intermediate = intermediate.Zip(source,
                            (values, next) =>
                            {
                                values.Add(next);
                                return values;
                            });
                }

                return intermediate?.Select(values => selector(values.AsReadOnly()))!;
            });
        }


        /// <summary>
        /// https://stackoverflow.com/questions/53152134/buffer-by-time-or-running-sum-for-reactive-extensions/53193641#53193641
        ///  answered Nov 7 '18 at 16:24
        /// Shlomo
        /// Buffer by size or timespan
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <param name="sizeSelector"></param>
        /// <param name="maxSize"></param>
        /// <param name="bufferTimeSpan"></param>
        /// <returns></returns>
        public static IObservable<IList<TSource?>> Buffer<TSource>(this IObservable<TSource> source, Func<TSource?, int> sizeSelector, int maxSize, TimeSpan bufferTimeSpan)
             where TSource : struct
        {
            BehaviorSubject<Unit> queue = new BehaviorSubject<Unit>(new Unit()); //our time-out mechanism

            return source
                .Union(queue.Delay(bufferTimeSpan))
                .ScanUnion(
                    (list: ImmutableList<TSource?>.Empty, size: 0, emitValue: (ImmutableList<TSource?>)null),
                    (state, item) =>
                    { // item handler
                        var itemSize = sizeSelector(item);
                        var newSize = state.size + itemSize;
                        if (newSize > maxSize)
                        {
                            queue.OnNext(Unit.Default);
                            return (ImmutableList<TSource?>.Empty.Add(item), itemSize, state.list);
                        }
                        else
                            return (state.list.Add(item), newSize, null);
                    },
                    (state, _) =>
                    { // time out handler
                        queue.OnNext(Unit.Default);
                        return (ImmutableList<TSource?>.Empty, 0, state.list);
                    }
                )
                .Where(t => t.emitValue != null)
                .Select(t => t.emitValue?.ToArray());
        }

        /// <summary>
        /// https://stackoverflow.com/questions/53152134/buffer-by-time-or-running-sum-for-reactive-extensions/53193641#53193641
        ///  answered Nov 7 '18 at 16:24
        /// Shlomo
        /// Buffer by size or timespan
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <param name="sizeSelector"></param>
        /// <param name="maxSize"></param>
        /// <param name="bufferTimeSpan"></param>
        /// <returns></returns>
        public static IObservable<IList<TSource>> Buffer<TSource>(this IObservable<TSource> source, Func<TSource, int> sizeSelector, int maxSize, TimeSpan bufferTimeSpan)
            where TSource : class
        {
            BehaviorSubject<Unit> queue = new BehaviorSubject<Unit>(new Unit()); //our time-out mechanism

            return source
                .Union(queue.Delay(bufferTimeSpan))
                .ScanUnion(
                    (list: ImmutableList<TSource>.Empty, size: 0, emitValue: (ImmutableList<TSource>)null),
                    (state, item) =>
                    { // item handler
                        var itemSize = sizeSelector(item);
                        var newSize = state.size + itemSize;
                        if (newSize > maxSize)
                        {
                            queue.OnNext(Unit.Default);
                            return (ImmutableList<TSource>.Empty.Add(item), itemSize, state.list);
                        }
                        else
                            return (state.list.Add(item), newSize, null);
                    },
                    (state, _) =>
                    { // time out handler
                        queue.OnNext(Unit.Default);
                        return (ImmutableList<TSource>.Empty, 0, state.list);
                    }
                )
                .Where(t => t.emitValue != null)
                .Select(t => t.emitValue?.ToArray());
        }
        /// <summary>
        /// <a href="https://stackoverflow.com/questions/7597773/does-reactive-extensions-support-rolling-buffers?noredirect=1"></a>
        /// answered Sep 30 '11 at 0:32
        /// Enigmativity
        /// </summary>
        public static IObservable<IEnumerable<T>> BufferWithInactivity<T>(
    this IObservable<T> source,
    TimeSpan inactivity,
    int maximumBufferSize)
        {
            return Observable.Create<IEnumerable<T>>(o =>
            {
                var gate = new object();
                var buffer = new List<T>();
                var mutable = new SerialDisposable();
                var subscription = (IDisposable)null;
                var scheduler = Scheduler.Default;

                Action dump = () =>
                {
                    var bts = buffer.ToArray();
                    buffer = new List<T>();
                    if (o != null)
                    {
                        o.OnNext(bts);
                    }
                };

                Action dispose = () =>
                {
                    if (subscription != null)
                    {
                        subscription.Dispose();
                    }
                    mutable.Dispose();
                };

                Action<Action<IObserver<IEnumerable<T>>>> onErrorOrCompleted =
                    onAction =>
                    {
                        lock (gate)
                        {
                            dispose();
                            dump();
                            if (o != null)
                            {
                                onAction(o);
                            }
                        }
                    };

                Action<Exception> onError = ex =>
                    onErrorOrCompleted(x => x.OnError(ex));

                Action onCompleted = () => onErrorOrCompleted(x => x.OnCompleted());

                Action<T> onNext = t =>
                {
                    lock (gate)
                    {
                        buffer.Add(t);
                        if (buffer.Count == maximumBufferSize)
                        {
                            dump();
                            mutable.Disposable = Disposable.Empty;
                        }
                        else
                        {
                            mutable.Disposable = scheduler.Schedule(inactivity, () =>
                            {
                                lock (gate)
                                {
                                    dump();
                                }
                            });
                        }
                    }
                };

                subscription =
                    source
                        .ObserveOn(scheduler)
                        .Subscribe(onNext, onError, onCompleted);

                return () =>
                {
                    lock (gate)
                    {
                        o = null!;
                        dispose();
                    }
                };
            });
        }
    }


    public static class DUnionExtensions
    {
        public class DUnion<T1, T2>
            where T1 : struct
            where T2 : struct
        {
            public DUnion(T1 t1)
            {
                Type1Item = t1;
                Type2Item = default;
                IsType1 = true;
            }

            public DUnion(T2 t2, bool ignored) //extra parameter to disambiguate in case T1 == T2
            {
                Type2Item = t2;
                Type1Item = default;
                IsType1 = false;
            }

            public bool IsType1 { get; }
            public bool IsType2 => !IsType1;

            public T1? Type1Item { get; }
            public T2? Type2Item { get; }
        }

        public static IObservable<DUnion<T1, T2>> Union<T1, T2>(this IObservable<T1> a, IObservable<T2> b)
                   where T1 : struct
            where T2 : struct
        {
            return a.Select(x => new DUnion<T1, T2>(x))
                .Merge(b.Select(x => new DUnion<T1, T2>(x, false)));
        }

        public static IObservable<TState> ScanUnion<T1, T2, TState>(this IObservable<DUnion<T1, T2>> source,
                TState initialState,
                Func<TState, T1?, TState> type1Handler,
                Func<TState, T2?, TState> type2Handler)
            where T1 : struct
            where T2 : struct
        {
            return source.Scan(initialState, (state, u) => u.IsType1
                ? type1Handler(state, u.Type1Item)
                : type2Handler(state, u.Type2Item)
            );
        }
    }

    public static class DUnionRefExtensions
    {
        public class DUnion<T1, T2>
            where T1 : struct
            where T2 : class
        {
            public DUnion(T1 t1)
            {
                Type1Item = t1;
                Type2Item = default;
                IsType1 = true;
            }

            public DUnion(T2 t2, bool ignored) //extra parameter to disambiguate in case T1 == T2
            {
                Type2Item = t2;
                Type1Item = default;
                IsType1 = false;
            }

            public bool IsType1 { get; }
            public bool IsType2 => !IsType1;

            public T1? Type1Item { get; }
            public T2 Type2Item { get; }
        }

        public static IObservable<DUnion<T1, T2>> Union<T1, T2>(this IObservable<T1> a, IObservable<T2> b)
                   where T1 : struct
            where T2 : class
        {
            return a.Select(x => new DUnion<T1, T2>(x))
                .Merge(b.Select(x => new DUnion<T1, T2>(x, false)));
        }

        public static IObservable<TState> ScanUnion<T1, T2, TState>(this IObservable<DUnion<T1, T2>> source,
                TState initialState,
                Func<TState, T1?, TState> type1Handler,
                Func<TState, T2, TState> type2Handler)
            where T1 : struct
            where T2 : class
        {
            return source.Scan(initialState, (state, u) => u.IsType1
                ? type1Handler(state, u.Type1Item)
                : type2Handler(state, u.Type2Item)
            );
        }
    }
    public static class DUnionRefExtensions2
    {
        public class DUnion<T1, T2>
            where T1 : class
            where T2 : struct
        {
            public DUnion(T1 t1)
            {
                Type1Item = t1;
                Type2Item = default;
                IsType1 = true;
            }

            public DUnion(T2 t2, bool ignored) //extra parameter to disambiguate in case T1 == T2
            {
                Type2Item = t2;
                Type1Item = default;
                IsType1 = false;
            }

            public bool IsType1 { get; }
            public bool IsType2 => !IsType1;

            public T1 Type1Item { get; }
            public T2? Type2Item { get; }
        }

        public static IObservable<DUnion<T1, T2>> Union<T1, T2>(this IObservable<T1> a, IObservable<T2> b)
                   where T1 : class
            where T2 : struct
        {
            return a.Select(x => new DUnion<T1, T2>(x))
                .Merge(b.Select(x => new DUnion<T1, T2>(x, false)));
        }

        public static IObservable<TState> ScanUnion<T1, T2, TState>(this IObservable<DUnion<T1, T2>> source,
                TState initialState,
                Func<TState, T1, TState> type1Handler,
                Func<TState, T2?, TState> type2Handler)
            where T1 : class
            where T2 : struct
        {
            return source.Scan(initialState, (state, u) => u.IsType1
                ? type1Handler(state, u.Type1Item)
                : type2Handler(state, u.Type2Item)
            );
        }
    }
}




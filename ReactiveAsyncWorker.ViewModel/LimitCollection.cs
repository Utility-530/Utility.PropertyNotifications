using DynamicData;
using ReactiveAsyncWorker.Model;
using ReactiveUI;
using System;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace ReactiveAsyncWorker.ViewModel
{

    public class LimitCollection : ReactiveObject
    {
        public virtual uint Count { get; }

        public virtual bool IsFree { get; }
    }


    public class LimitCollection<T> : LimitCollection, IBasicCollection<T>, IObserver<Capacity>, IObservable<T>
        where T : UtilityInterface.Generic.IKey<string>, IEquatable<T>, IComparable<T>
    {
        readonly ReplaySubject<Capacity> capacitySubject = new ReplaySubject<Capacity>();
        readonly ReplaySubject<T> nextSubject = new ReplaySubject<T>();
        readonly SourceCache<T, string> initialSourceCache = new SourceCache<T, string>(a => a.Key);
        readonly SourceCache<T, string> finalSourceCache = new SourceCache<T, string>(a => a.Key);

        private readonly ObservableAsPropertyHelper<bool> isFree;
        private readonly ObservableAsPropertyHelper<Capacity> count;

        public LimitCollection(IObservable<Capacity>? observable = default)
        {
            observable?.Subscribe(capacitySubject.OnNext);

            var obs = initialSourceCache.CountChanged.SelectAdditions();
            var obs2 = finalSourceCache.CountChanged.SelectSubtractions();

            var dis = obs
                .CombineLatest(capacitySubject.StartWith(new Capacity(1)), obs2.StartWith(0))
                .Subscribe(d =>
                {
                    var (a, b, c) = d;

                    // SourceCache items are not ordered
                    if (initialSourceCache.Items.OrderBy(a => a).FirstOrDefault() is { } item &&
                       finalSourceCache.Count < b.Value)
                    {
                        finalSourceCache.AddOrUpdate(item);
                        nextSubject.OnNext(item);
                        initialSourceCache.RemoveKey(item.Key);
                    }
                });

            isFree = finalSourceCache.CountChanged.CombineLatest(
                initialSourceCache.CountChanged,
                capacitySubject.StartWith(new Capacity(1)), (a, c, b) => (a + c) < b.Value)
                .ToProperty(this, a => a.IsFree);

            isFree.ThrownExceptions.Subscribe(a =>
            {

            });

            count = capacitySubject.ToProperty(this, nameof(Count));
        }

        public void Remove(T obj)
        {
            var count = finalSourceCache.Count;
            try
            {
                lock (finalSourceCache)
                    finalSourceCache.RemoveKey(obj.Key);
            }
            catch (Exception ex)
            {

            }

            if (finalSourceCache.Count == count)
            {

            }
        }

        public void Add(T obj)
        {
            initialSourceCache.AddOrUpdate(obj);
        }

        public override uint Count => count.Value.Value;

        public override bool IsFree => isFree.Value;

        public IDisposable Subscribe(IObserver<T> observer)
        {
            return nextSubject.Subscribe(observer);
        }

        public void OnCompleted()
        {
            throw new NotImplementedException();
        }

        public void OnError(Exception error)
        {
            throw new NotImplementedException();
        }

        public void OnNext(Capacity value)
        {
            capacitySubject.OnNext(value);
        }
    }
}

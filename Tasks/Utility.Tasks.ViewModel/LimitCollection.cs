using DynamicData;
using Utility.Tasks.Model;
using ReactiveUI;
using System;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Collections.Generic;
using System.Text;
using Kaos.Collections;

namespace Utility.Tasks.ViewModel
{
    /// <summary>
    /// Observable class that only notifies subscribers of items in the 
    /// collection when the limit is not exceeded by the number of items added.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class LimitCollection<T> : LimitCollection, IBasicObservableCollection<T>, IObserver<Capacity>, IObservable<T>
        where T : UtilityInterface.Generic.IKey<string>, IEquatable<T>, IComparable<T>
    {
        readonly ReplaySubject<Capacity> capacitySubject = new();
        readonly ReplaySubject<T> nextSubject = new();
        readonly SourceCache<T, string> initialSourceCache = new(a => a.Key);
        readonly SourceCache<T, string> finalSourceCache = new(a => a.Key);
        readonly SortedSet<T> hashSet = new(/*EqualityComparer<T>.Create((a,b)=>a )*/);

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

                    lock (finalSourceCache)
                        if (hashSet.FirstOrDefault() is { } item &&
                           // Only notify subscribers when the limit is not exceeded.
                           finalSourceCache.Count < b.Value)
                        {
                            finalSourceCache.AddOrUpdate(item);
                            nextSubject.OnNext(item);
                            initialSourceCache.RemoveKey(item.Key);
                            hashSet.Remove(item);
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


        int i = 0;

        int digit = 0;

        public void Remove(T obj)
        {
            i++;

            lock (finalSourceCache)
            {
                var keys = finalSourceCache.Keys;
                if (finalSourceCache.Count > count.Value.Value)
                {
                    throw new Exception("sdfdeRTT");
                }
                finalSourceCache.RemoveKey(obj.Key);
                var endKeys = finalSourceCache.Keys;

                if (endKeys.SequenceEqual(keys))
                {

                }
            }
        }

        //int adigit = 0;
        public void Add(T obj)
        {
            //var newDigit = int.Parse(obj.Key.First().ToString());
            //if (newDigit <= adigit)
            //{

            //}
            // SourceCache items are not ordered, hence secondary collection that retains the order
            hashSet.Add(obj);
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

    public class LimitCollection : ReactiveObject
    {
        public virtual uint Count { get; }

        public virtual bool IsFree { get; }
    }

}

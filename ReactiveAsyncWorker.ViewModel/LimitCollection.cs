using DynamicData;
using ReactiveAsyncWorker.Model;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;

namespace ReactiveAsyncWorker.ViewModel
{

    public class LimitCollection : ReactiveObject
    {
        public virtual uint Count { get; }

        public virtual bool IsFree { get; }
    }


    public class LimitCollection<T> : LimitCollection, IBasicCollection<T>, IObserver<Capacity>, IObservable<T> where T : UtilityInterface.Generic.IKey<string>, IEquatable<T>
    {
        readonly ReplaySubject<Capacity> capacitySubject = new ReplaySubject<Capacity>();
        readonly ReplaySubject<T> nextSubject = new ReplaySubject<T>();
        readonly SourceCache<T, string> cacheQueue = new SourceCache<T, string>(a => a.Key);

        private readonly ObservableAsPropertyHelper<bool> isFree;
        private readonly ObservableAsPropertyHelper<Capacity> count;

        public LimitCollection(IObservable<Capacity>? observable = default)
        {
            //parralelisationSubject.Subscribe(capacitySubject.OnNext(capacity));

            observable?.Subscribe(capacitySubject.OnNext);

            var dis = cacheQueue.CountChanged
                            .StartWith(0)
                            .DistinctUntilChanged()
                             .CombineLatest(capacitySubject.StartWith(new Capacity((uint)1)), (a, b) => a < b.Value)
                             .Where(a => a)
                             .Select(a => cacheQueue.Items.FirstOrDefault())
                             .Subscribe(nextSubject.OnNext);


            isFree = nextSubject.Select(a => a?.Equals(default) == false).ToProperty(this, nameof(IsFree));

            count = capacitySubject.ToProperty(this, nameof(Count));
        }

        public void Remove(T obj)
        {
            cacheQueue.RemoveKey(obj.Key);
        }

        public void Add(T obj)
        {

            cacheQueue.AddOrUpdate(obj);
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

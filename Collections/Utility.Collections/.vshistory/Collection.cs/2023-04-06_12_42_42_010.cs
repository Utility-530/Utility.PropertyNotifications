namespace Models
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;
    using Utility.Observables;

    public class Collection : SortableObservableCollection<object>, IObservable
    {
        DeferredEventsCollection _deferredEvents;
        public List<IObserver> Observers { get; } = new();

        public Collection()
        {
        }

        public Collection(IEnumerable<object> collection, IComparer<object> comparer = null) : base(collection, comparer)
        {
        }

        public Collection(List<object> list, IComparer<object> comparer = null) : base(list, comparer)
        {
        }


        /// <summary>
        /// Raise CollectionChanged event to any listeners.
        /// Properties/methods modifying this ObservableCollection will raise
        /// a collection changed event through this virtual method.
        /// </summary>
        /// <remarks>
        /// When overriding this method, either call its base implementation
        /// or call <see cref="BlockReentrancy"/> to guard against reentrant collection changes.
        /// </remarks>
        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            var _deferredEvents = typeof(RangeObservableCollection<object>).GetField("_deferredEvents", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(this);
            if (_deferredEvents is IList list)
            {
                list.Add(e);
                return;
            }

            foreach (var handler in GetHandlers())
                if (IsRange(e) && handler.Target.HasRefresh())
                    handler.Target.Refresh();
                else
                    handler(this, e);

            foreach (var observer in Observers)
            {
                observer.OnNext(e);
            }
        }


        public void Complete()
        {
            foreach (var observer in Observers.ToArray())
            {
                observer.OnCompleted();
            }
        }

        public IDisposable Subscribe(IObserver observer)
        {

            observer.OnNext(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, this));
            return new Disposer(Observers, observer);
        }


        protected override IDisposable DeferEvents() => new DeferredEventsCollection(this);

        bool IsRange(NotifyCollectionChangedEventArgs e) => e.NewItems?.Count > 1 || e.OldItems?.Count > 1;

        IEnumerable<NotifyCollectionChangedEventHandler> GetHandlers()
        {
            var info = typeof(ObservableCollection<object>).GetField(nameof(CollectionChanged), BindingFlags.Instance | BindingFlags.NonPublic);
            var @event = (MulticastDelegate)info.GetValue(this);
            return @event?.GetInvocationList()
              .Cast<NotifyCollectionChangedEventHandler>()
              .Distinct()
              ?? Enumerable.Empty<NotifyCollectionChangedEventHandler>();
        }

        /// <summary>
        /// <a href="https://gist.github.com/weitzhandler/"></a>
        /// </summary>
        class DeferredEventsCollection : List<NotifyCollectionChangedEventArgs>, IDisposable
        {
            private readonly Collection _collection;
            public DeferredEventsCollection(Collection collection)
            {
                Debug.Assert(collection != null);
                Debug.Assert(collection._deferredEvents == null);
                _collection = collection;
                _collection._deferredEvents = this;
            }


            public void Dispose()
            {
                _collection._deferredEvents = null;

                var handlers = _collection
                  .GetHandlers()
                  .ToLookup(h => h.Target?.HasRefresh());

                foreach (var handler in handlers[false])
                    foreach (var e in this)
                        handler(_collection, e);

                foreach (var cv in handlers[true].Select(h => h.Target)
                  //.Cast<CollectionView>()
                  .Distinct())
                {
                    cv?.Refresh();
                }
            }
        }

    }

    public static class RefreshHelper
    {

        const string MethodName = "Refresh";

        public static bool HasRefresh(this object objectToCheck)
        {
            var type = objectToCheck.GetType();
            return type.GetMethod(MethodName) != null;
        }



        public static void Refresh(this object objectToCheck)
        {
            var methodInfo = objectToCheck.GetType().GetMethod(MethodName);
            methodInfo.Invoke(objectToCheck, null);

        }
    }
}
using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace ReactiveAsyncWorker
{
    public class WorkerItem<TOutput> : StatusItem, IWorkerItem<TOutput>
    {
        protected readonly ISubject<TOutput> stateChanges = new ReplaySubject<TOutput>();

        public WorkerItem(string key, IObservable<TOutput> action) : base(key)
        {            
            action
                .Take(1)
                .Subscribe(state => State = state,
                e =>
                {

                }, () =>
                {
                    completed.OnNext(Unit.Default);
                    stateChanges.OnNext(State);                   
                });
        }

        public IObservable<TOutput> StateChanges => stateChanges.AsObservable();

        public virtual TOutput State { get; protected set; }

        public virtual void Start()
        {
            started.OnNext(Unit.Default);            
        }
    }
}

using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace ReactiveAsyncWorker
{
    public class WorkerItem<TOutput> : StatusItem, IWorkerItem<TOutput>
    {
        private readonly IObservable<TOutput> action;
        protected readonly ISubject<TOutput> stateChanges = new ReplaySubject<TOutput>();

        public WorkerItem(string key, IObservable<TOutput> action) : base(key)
        {
            this.action = action;
        }

        public IObservable<TOutput> StateChanges => stateChanges.AsObservable();

        public virtual TOutput State { get; protected set; }

        public virtual void Start()
        {
            started.OnNext(Unit.Default);
            action
                .Take(1)
                .Subscribe(state => State = state, () =>
            {
                stateChanges.OnNext(State);
                completed.OnNext(Unit.Default);
            });
        }
    }
}

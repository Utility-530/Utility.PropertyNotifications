using ReactiveAsyncWorker.Model;
using ReactiveUI;
using System;
using System.ComponentModel;
using System.ComponentModel.Custom.Generic;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using UtilityEnum;

namespace ReactiveAsyncWorker.ViewModel
{
    public class ProgressWorkerItem : ProgressWorkerItem<object>
    {
        public ProgressWorkerItem(IObservable<ProgressChangedEventArgs> progress, IObservable<AsyncCompletedEventArgs> completion, Action actn, string key) :
         base(progress.Select(a => new ProgressChangedEventArgs<object>(a.ProgressPercentage, a.UserState)), completion, actn, key)
        {
        }
    }

    public class ProgressWorkerItem<T> : Progress, IWorkerItem<T>
    {
        private readonly Action actn;
        private readonly ISubject<Unit> started = new ReplaySubject<Unit>();
        private readonly ObservableAsPropertyHelper<T> progressState;
        private readonly ObservableAsPropertyHelper<ProcessState> status;

        public ProgressWorkerItem(IObservable<ProgressChangedEventArgs<T>> progress, IObservable<AsyncCompletedEventArgs> completed, Action actn, string key) :
            base(progress.Select(p => p.ProgressPercentage).TakeUntil(completed))
        {
            status = completed
            .Select(a => ProcessState.Terminated)
            .Merge(started.Select(a => ProcessState.Running))
            .ToProperty(this, a => a.Status, initialValue: ProcessState.Ready);

            StateChanges = completed.Select(a => (T)a.UserState);

            progressState = progress.Select(p => p.UserState).TakeUntil(StateChanges).CombineLatest(started, (a, b) => a)
                .ToProperty(this, a => a.ProgressState);

            // Result = completion.TakeUntil(Completed).CombineLatest(Started, (a, b) => (T)a.UserState).ToReadOnlyReactiveProperty();

            this.actn = actn;
            Key = key;
            // StateChanges.Subscribe(c => IsCompleted = true);

            changes =
                completed.Select(a => new ProgressState(Key, ProcessState.Terminated, 1, false))
            .Merge(started.Select(a => new ProgressState(Key, ProcessState.Running, 0, false))
            .Merge(progress.Select(a => new ProgressState(Key, ProcessState.Running, a.ProgressPercentage / 100d, false))))
            .StartWith(new ProgressState(Key, ProcessState.Ready, 0, false));

            status = changes.Select(a => a.State).ToProperty(this, a => a.Status, initialValue: ProcessState.Ready);

        }

        public string Key { get; }

        private readonly IObservable<ProgressState> changes;

        public T ProgressState => progressState.Value;

        public IObservable<T> StateChanges { get; }

        //  public IObservable<Unit> Started => started.AsObservable();

        public T State { get; }

        // public bool IsCompleted { get; private set; }

        public ProcessState Status => status.Value;

        public void Start()
        {
            started.OnNext(Unit.Default);
            actn.Invoke();
        }

        public IDisposable Subscribe(IObserver<ProgressState> observer)
        {
            return changes.Subscribe(observer);
        }
    }


    public abstract class Progress : ReactiveObject, IProgress
    {
        private readonly ObservableAsPropertyHelper<int> progressPercent;

        public Progress(IObservable<int> progress)
        {
            progressPercent = progress.ToProperty(this, a => a.ProgressPercent);
        }

        public int ProgressPercent => progressPercent.Value;

    }
}


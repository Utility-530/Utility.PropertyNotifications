using Utility.Tasks.Model;
using ReactiveUI;
using System;
using System.ComponentModel;
using System.ComponentModel.Custom.Generic;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using UtilityEnum;

namespace Utility.Tasks.ViewModel
{
    public class ProgressWorkerItem : ProgressWorkerItem<object>
    {
        public ProgressWorkerItem(IObservable<ProgressChangedEventArgs> progress, IObservable<AsyncCompletedEventArgs> completion, Action actn, string key) :
         base(progress.Select(a => new ProgressChangedEventArgs<object>(a.ProgressPercentage, a.UserState)), completion, actn, key)
        {
        }
    }

    public record AsyncEventArgs<T>(ProgressChangedEventArgs<T>? progressChangedEventArgs, AsyncCompletedEventArgs? CompletedEventArgs );
   

    public record AsyncTaskOutput<T> : TaskOutput
    {
        public AsyncTaskOutput(string key, AsyncEventArgs<T> value) : base(key, value)
        {
            Value = value;
        }

        public override AsyncEventArgs<T> Value { get; }

    }

    public class ProgressWorkerItem<T> : Progress, IWorkerItem
    {
        private readonly Action actn;
        private readonly ISubject<Unit> started = new ReplaySubject<Unit>();
        private readonly ObservableAsPropertyHelper<T> progressState;
        private readonly ObservableAsPropertyHelper<ProcessState> status;
        private readonly IObservable<ProgressState> changes;
        private DateTime startTime;


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
                completed.Select(a => new ProgressState(Key, ProcessState.Terminated, new ProgressItem(1, false, false, default, startTime), new AsyncTaskOutput<T>(Key, new AsyncEventArgs<T>(default, a))))
            .Merge(
             started.Select(a => new ProgressState(Key, ProcessState.Running, new ProgressItem(0, false, false, default, startTime), new AsyncTaskOutput<T>(Key, new AsyncEventArgs<T>(default, default))))
            .Merge(
             progress.Select(a => new ProgressState(Key, ProcessState.Running, new ProgressItem(a.ProgressPercentage / 100d, false, false, default, startTime), new AsyncTaskOutput<T>(Key, new AsyncEventArgs<T>(a, default))))))
            .StartWith(new ProgressState(Key, ProcessState.Ready, new ProgressItem(0, false, false, default, startTime), new AsyncTaskOutput<T>(Key, new AsyncEventArgs<T>(default, default))));

            status = changes.Select(a => a.State).ToProperty(this, a => a.Status, initialValue: ProcessState.Ready);
        }

        public string Key { get; }


        public T ProgressState => progressState.Value;

        public IObservable<T> StateChanges { get; }

        public T State { get; }

        public ProcessState Status => status.Value;

        public void Start()
        {
            startTime = DateTime.Now;
            started.OnNext(Unit.Default);
            actn.Invoke();
        }

        public IDisposable Subscribe(IObserver<IProgressState> observer)
        {
            return changes.Subscribe(observer);
        }

        public void Pause()
        {
            throw new NotImplementedException();
        }

        public void Stop()
        {
            throw new NotImplementedException();
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


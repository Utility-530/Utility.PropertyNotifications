using UtilityInterface.Generic;
using ReactiveAsyncWorker.Model;
using ReactiveUI;
using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using UtilityEnum;

namespace ReactiveAsyncWorker
{
    public class StatusItem : ReactiveObject, IObservable<ProgressState>, IKey<string>
    {
        protected readonly ISubject<Unit> completed = new ReplaySubject<Unit>();
        protected readonly ISubject<Unit> started = new ReplaySubject<Unit>();
        protected readonly IConnectableObservable<ProgressState> changes;
        protected readonly ObservableAsPropertyHelper<ProcessState> status;

        public StatusItem(string key)
        {
            Key = key;
            changes = completed
                       .Select(a => ProcessState.Terminated)
                       .Merge(started.Select(a => ProcessState.Running))
                       .Select(processState => new ProgressState(this.Key,
                           processState,
                           processState switch
                           {
                               ProcessState.Ready => 0,
                               ProcessState.Terminated => 1,
                               ProcessState.Running => 0.5,
                               _ => throw new NotImplementedException()
                           },
                           processState == ProcessState.Running))
                       .StartWith(new ProgressState(this.Key, ProcessState.Ready, 0, false))
                       .Replay();

            changes.Connect();
            status = changes.Select(a => a.State).ToProperty(this, a => a.Status, initialValue: ProcessState.Ready);
        }

        public string Key { get; }

        public ProcessState Status => status.Value;

        public IDisposable Subscribe(IObserver<ProgressState> observer)
        {
            //changes.Where(a =>
            //{
            //    return a.State == ProcessState.Terminated;
            //}).Subscribe(a =>
            //{
            //    observer.OnCompleted();
            //});
            return changes.Subscribe(observer);
        }
    }
}

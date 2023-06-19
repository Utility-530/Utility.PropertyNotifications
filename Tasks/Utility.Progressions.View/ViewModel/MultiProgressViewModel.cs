using DynamicData;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Utility.Progressions;

namespace Utility.Progressions
{
    public class MultiProgressViewModel : ReactiveObject, IObserver<IProgressState>
    {
        protected readonly ReplaySubject<IProgressState> progressTasks = new();
        private readonly ReadOnlyObservableCollection<ProgressViewModel> collection;
        private readonly ObservableAsPropertyHelper<ProgressViewModel> current;

        public MultiProgressViewModel(IEnumerable<IObservable<IProgressState>> manyProgress, IScheduler? scheduler = default)
        {
            var progress = manyProgress
                .ToObservable()
                .SelectMany(a => a)
                .ObserveOn(scheduler ?? RxApp.MainThreadScheduler)
                .Subscribe(progressTasks.OnNext);

            var changeSet = ObservableChangeSet.Create<IProgressState, string>(observableCache =>
            {
                CompositeDisposable disposable = new();
                progressTasks
                .Subscribe(a =>
                {
                    observableCache.AddOrUpdate(a);
                })
                .DisposeWith(disposable);

                observableCache.LimitSizeTo(5);
                //progressTasks
                //.Where(a => a.Progress.IsComplete == true)
                //.SelectMany(a => Observable.Timer(TimeSpan.FromSeconds(2)).Select(_ => a.Key))
                //.ObserveOn(scheduler ?? RxApp.MainThreadScheduler)
                //.Subscribe(a => observableCache.RemoveKey(a))
                //.DisposeWith(disposable);

                return disposable;
            }, trade => trade.Key)
            .Transform(a => new ProgressViewModel(a.Progress));

            current = changeSet
                .ToSortedCollection(a => a.Started)
                .Select(a => a.First())
                .ToProperty(this, a => a.Current);

            changeSet.Bind(out collection).Subscribe();
        }

        public string? Key { get; }

        public ReadOnlyObservableCollection<ProgressViewModel> Collection => collection;

        public ProgressViewModel Current => current.Value;

        public void OnCompleted()
        {
            throw new NotImplementedException();
        }

        public void OnError(Exception error)
        {
            throw new NotImplementedException();
        }

        public void OnNext(IProgressState value)
        {
            progressTasks.OnNext(value);
        }
    }
}
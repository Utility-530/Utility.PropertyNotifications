using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Collections.ObjectModel;
using System.Reactive.Concurrency;
using DynamicData;
using UtilityEnum;
using ReactiveUI;
using ReactiveAsyncWorker.Model;
using DynamicData.Binding;
using System.Collections;

namespace ReactiveAsyncWorker.ViewModel
{
    public class AsyncTaskStatusViewModel : ReactiveObject, IObserver<ProgressState>
    {
        private readonly CollectionViewModel<ProgressState, string> readyViewModel;
        private readonly CollectionViewModel<ProgressState, string> runningViewModel;
        private readonly CollectionViewModel<ProgressState, string> terminatedViewModel;
        private readonly ReadOnlyObservableCollection<ReactiveProcessPair> items;
        private readonly ReadOnlyObservableCollection<KeyCollection> keyGroups;
        private readonly ISubject<ProgressState> progressStateSubject = new ReplaySubject<ProgressState>();


        public AsyncTaskStatusViewModel(IObservable<ProgressState> progressState, IScheduler scheduler)
        {
            progressState.Subscribe(progressStateSubject.OnNext);

            progressStateSubject
                .ObserveOn(scheduler)
                .SubscribeOn(scheduler)
                .ToObservableChangeSet()
                .GroupOn(a => a.Key)
                .Transform(a =>
                {
                    a.List.Connect().Bind(out var gitems).Subscribe();
                    return new KeyCollection(a.GroupKey, gitems);
                })
                .Bind(out keyGroups)
                .Subscribe();

            var transforms = ChangeSetHelper.SelectGroups<ProgressState, ProcessState, string>(progressStateSubject, scheduler);

            transforms
                .Transform(a => ReactiveProcessPair.Create(a.Key, a.Cache.Items))
                .Bind(out items)
                .Subscribe();

            readyViewModel = CollectionViewModel.Create(nameof(ProcessState.Ready),
                transforms.FilterAndSelect(a => a.Key == ProcessState.Ready, a => a.Key, a => a?.Cache));


            runningViewModel = CollectionViewModel.Create(nameof(ProcessState.Running),
                transforms.FilterAndSelect(a => a.Key == ProcessState.Running, a => a.Key, a => a?.Cache));

            terminatedViewModel = CollectionViewModel.Create(nameof(ProcessState.Terminated), 
                transforms.FilterAndSelect(a => a.Key == ProcessState.Terminated, a => a.Key, a => a?.Cache));

        }


        public CollectionViewModel ReadyViewModel => readyViewModel;

        public CollectionViewModel RunningViewModel => runningViewModel;

        public CollectionViewModel TerminatedViewModel => terminatedViewModel;

        public ReadOnlyObservableCollection<ReactiveProcessPair> CombinedCollection => items;

        public ReadOnlyObservableCollection<KeyCollection> GroupedCollection => keyGroups;


        public void OnCompleted()
        {
            throw new NotImplementedException();
        }

        public void OnError(Exception error)
        {
            throw new NotImplementedException();
        }

        public void OnNext(ProgressState value)
        {
            progressStateSubject.OnNext(value);
        }
    }

    public class ReactiveProcessPair : ReactivePair<ProcessState, IEnumerable<ProgressState>>
    {
        public ReactiveProcessPair(ProcessState key, IEnumerable<ProgressState> value) : base(key, value)
        {
        }

        public static new ReactiveProcessPair Create(ProcessState key, IEnumerable<ProgressState> value)
        {
            return new ReactiveProcessPair(key, value);
        }
    }

}

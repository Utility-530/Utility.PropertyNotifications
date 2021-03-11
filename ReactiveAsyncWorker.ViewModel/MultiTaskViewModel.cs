using System;
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
using Endless;

namespace ReactiveAsyncWorker.ViewModel
{
    public class MultiTaskViewModel : ReactiveObject, IObserver<ProgressState>
    {
        private readonly CollectionViewModel<ProgressState, string> readyViewModel;
        private readonly CollectionViewModel<ProgressState, string> runningViewModel;
        private readonly CollectionViewModel<ProgressState,  string> terminatedViewModel;
        private readonly CollectionViewModel<ProgressState,  string> latestViewModel;
        private readonly CollectionViewModel<KeyCollection, ProcessState> combinedItems;
        private readonly CollectionViewModel<KeyCollection> keyGroups;
        private readonly ISubject<ProgressState> progressStateSubject = new ReplaySubject<ProgressState>();

        public MultiTaskViewModel(IObservable<ProgressState> progressState, IScheduler scheduler)
        {
            progressState.Subscribe(progressStateSubject.OnNext);

            keyGroups = CollectionViewModel
                            .Create("Keys",ChangeSetHelper.SelectKeyGroups(progressStateSubject, scheduler, a => new ProgressStateSummary(a.Key, a.State, a.Date)));

            var transforms = ChangeSetHelper
                            .SelectGroups<ProgressState, ProcessState, string>(progressStateSubject, scheduler);

            latestViewModel = CollectionViewModel.Create("All", progressStateSubject.ToObservableChangeSet(a => a.Key));

            combinedItems = CollectionViewModel.Create("Combined", ChangeSetHelper
                     .SelectGroupGroups2<ProgressState,ProcessState, ProgressStateSummary>(progressStateSubject, scheduler,
                     a=>new ProgressStateSummary(a.Key, a.State, a.Date)));

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

        public CollectionViewModel LatestViewModel => latestViewModel;

        public CollectionViewModel CombinedCollection => combinedItems;

        public CollectionViewModel GroupedCollection => keyGroups;


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
}

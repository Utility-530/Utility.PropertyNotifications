using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reactive.Concurrency;
using DynamicData;
using UtilityEnum;
using ReactiveUI;
using Utility.Tasks.Model;
using DynamicData.Binding;
using Utility.ViewModel;
using System.Linq;
using Utility.Infrastructure;

namespace Utility.Tasks.ViewModel
{
    public class MultiTaskViewModel : ReactiveObject, IObserver<IProgressState>, IObservable<TaskChangeRequest>
    {
        private readonly CollectionViewModel<ProgressViewModel> viewModels;
        private readonly CollectionViewModel<IProgressState, string> createdViewModel;
        private readonly CollectionViewModel<IProgressState, string> readyViewModel;
        private readonly CollectionViewModel<IProgressState, string> runningViewModel;
        private readonly CollectionViewModel<IProgressState, string> terminatedViewModel;
        private readonly CollectionViewModel<IProgressState, string> allItems;
        private readonly CollectionViewModel<KeyCollection, ProcessState> combinedItems;
        private readonly CollectionViewModel<KeyCollection> keyGroups;
        private readonly ReplaySubject<IProgressState> progressStateSubject = new();
        private readonly ReplaySubject<TaskChangeRequest> taskChangesSubject = new();

        public MultiTaskViewModel(IObservable<IProgressState> progressState, IScheduler scheduler)
        {
            progressState.ObserveOn(scheduler).Subscribe(progressStateSubject.OnNext);

            keyGroups = CollectionViewModel
                            .Create("Keys", ChangeSetHelper.SelectKeyGroups(progressStateSubject, scheduler, a => new ProgressStateSummary(a.Key, a.State, a.Progress.Started)));

            var transforms = ChangeSetHelper
                            .SelectGroups<IProgressState, ProcessState, string>(progressStateSubject, scheduler);

            var transforms2 = ChangeSetHelper
                            .SelectGroups<IProgressState, string>(progressStateSubject, scheduler);

            //progressStateSubject.Subscribe(a =>
            //{
            //    System.Diagnostics.Debug.WriteLine(a.Key + " " + a.State);
            //});
 

            allItems = CollectionViewModel.Create("All", progressStateSubject.ToObservableChangeSet(a => a.Key));

            combinedItems = CollectionViewModel.Create("Combined", ChangeSetHelper
                     .SelectGroupKeyGroups<IProgressState, ProcessState, ProgressStateSummary>(progressStateSubject, scheduler,
                     a => new ProgressStateSummary(a.Key, a.State, a.Progress.Started)));


            var viewModelsTemp = transforms2.Transform(a =>
            {
                //var items = a.ObservableList.Connect().ToSortedCollection(a => a.Progress.CurrentTime).Select(a => a.Last());
                //a.ObservableList.Connect().Transform(a=> new ProgressStateSummary(a.Key, a.State, a.Progress.Started)).Bind(out var gitems).Subscribe();

                var progressViewModel = new ProgressViewModel(a.Key,  a.ObservableList.Connect());
                progressViewModel.Subscribe(taskChangesSubject);
                return progressViewModel;
            });


            viewModels = CollectionViewModel.Create("ViewModels", viewModelsTemp);

            createdViewModel = CollectionViewModel.Create(nameof(ProcessState.Created),
          transforms.FilterAndSelect(a => a.Key == ProcessState.Created, a => a.Key, a => a?.Cache));

            readyViewModel = CollectionViewModel.Create(nameof(ProcessState.Ready),
                transforms.FilterAndSelect(a => a.Key == ProcessState.Ready, a => a.Key, a => a?.Cache));

            runningViewModel = CollectionViewModel.Create(nameof(ProcessState.Running),
                transforms.FilterAndSelect(a => a.Key == ProcessState.Running, a => a.Key, a => a?.Cache));

            terminatedViewModel = CollectionViewModel.Create(nameof(ProcessState.Terminated),
                transforms.FilterAndSelect(a => a.Key == ProcessState.Terminated, a => a.Key, a => a?.Cache));
        }

        public CollectionViewModel ViewModels => viewModels;

        public CollectionViewModel CreatedViewModel => createdViewModel;

        public CollectionViewModel ReadyViewModel => readyViewModel;

        public CollectionViewModel RunningViewModel => runningViewModel;

        public CollectionViewModel TerminatedViewModel => terminatedViewModel;

        public CollectionViewModel LatestCollection => allItems;

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

        public void OnNext(IProgressState value)
        {
            progressStateSubject.OnNext(value);
        }

        public IDisposable Subscribe(IObserver<TaskChangeRequest> observer)
        {
            return taskChangesSubject.Subscribe(observer);
        }
    }

    public class ProgressStateSummary
    {
        public ProgressStateSummary(string key, ProcessState state, DateTime date = default)
        {
            Key = key;
            State = state;
            Date = date.Equals(default) ? DateTime.Now : date;
        }

        public string Key { get; }

        public DateTime Date { get; }

        public ProcessState State { get; }
    }
}

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
using Endless;

namespace ReactiveAsyncWorker.ViewModel
{
    public class MultiTaskViewModel : ReactiveObject, IObserver<ProgressState>
    {
        private readonly CollectionViewModel<ProgressState, string> readyViewModel;
        private readonly CollectionViewModel<ProgressState, string> runningViewModel;
        private readonly CollectionViewModel<ProgressState,  string> terminatedViewModel;
        private readonly ReadOnlyObservableCollection<KeyCollection> combinedItems;
        private readonly ReadOnlyObservableCollection<KeyCollection> keyGroups;
        private readonly ISubject<ProgressState> progressStateSubject = new ReplaySubject<ProgressState>();

        public MultiTaskViewModel(IObservable<ProgressState> progressState, IScheduler scheduler)
        {
            progressState.Subscribe(progressStateSubject.OnNext);

            keyGroups = ChangeSetHelper.SelectKeyGroups(progressStateSubject, scheduler, a => new ProgressStateSummary(a.Key, a.State, a.Date), out _);

            var transforms = ChangeSetHelper
                            .SelectGroups<ProgressState, ProcessState, string>(progressStateSubject, scheduler);
                           

            combinedItems = ChangeSetHelper
                     .SelectGroupGroups2<ProgressState,ProcessState, ProgressStateSummary>(progressStateSubject, scheduler,
                     a=>new ProgressStateSummary(a.Key, a.State, a.Date), out _);

            //transforms2
            //  //.Transform(a => ReactiveProcessPair.Create(a.Key, a.Cache.Items))
            //  .Bind(out combinedItems)
            //  .Subscribe();

            readyViewModel = CollectionViewModel.Create(nameof(ProcessState.Ready),
                transforms.FilterAndSelect(a => a.Key == ProcessState.Ready, a => a.Key, a => a?.Cache));

            runningViewModel = CollectionViewModel.Create(nameof(ProcessState.Running),
                transforms.FilterAndSelect(a => a.Key == ProcessState.Running, a => a.Key, a => a?.Cache));

            terminatedViewModel = CollectionViewModel.Create(nameof(ProcessState.Terminated),
                transforms.FilterAndSelect(a => a.Key == ProcessState.Terminated, a => a.Key, a => a?.Cache));
        }

        public static IObservable<IChangeSet<TOut, TKey>>
    FilterAndSelect<T, TKey, TOut>(
    IObservable<IChangeSet<T, TKey>> observable,
    Func<T, bool> predicate,
    Func<TOut, TKey> keySelector,
    Func<T?, IObservableCache<TOut, TKey>?> selector)
        {
            var collection =
                observable
            .Filter(a => predicate(a))
            .ToCollection()
            .WhereNotNull()
            .Select(a => selector(a.FirstOrDefault()) ?? new SourceCache<TOut, TKey>(keySelector))
            .SelectMany(a => a.Connect());

            return collection;
        }


        public CollectionViewModel ReadyViewModel => readyViewModel;

        public CollectionViewModel RunningViewModel => runningViewModel;

        public CollectionViewModel TerminatedViewModel => terminatedViewModel;

        public ReadOnlyObservableCollection<KeyCollection> CombinedCollection => combinedItems;

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

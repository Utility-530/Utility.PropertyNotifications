using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Windows.Input;
using DynamicData;
using ReactiveUI;

using Utility.Infrastructure;
using Utility.Enums;
using Utility.Progressions;

namespace Utility.Tasks.ViewModel
{

    public class ProgressViewModel : ReactiveObject, IObservable<TaskChangeRequest>, IComparable<ProgressViewModel>, IComparable
    {
        private readonly ObservableAsPropertyHelper<ProcessState> _state;
        private readonly ObservableAsPropertyHelper<ProgressItem> _progress;
        private readonly ReplaySubject<TaskChangeRequest> requests = new();
        private readonly ObservableAsPropertyHelper<bool> hasPotentialProgress;
        private readonly ReadOnlyObservableCollection<ProgressStateSummary> progressStatesCollection;

        public ProgressViewModel(string key, 
            IObservable<IChangeSet<IProgressState>> progressState)
        {
            Key = key;

            var command = ReactiveCommand.Create<Unit, Unit>(_ => _);

            var items = progressState.ToSortedCollection(a => a.Progress.CurrentTime).Select(a => a.Last());
           
            _state = items.Select(a => a.State).ToProperty(this, a => a.State);
            _progress = items.Select(a => a.Progress).ToProperty(this, a => a.Progress);

            _ = command.WithLatestFrom(items.Select(a => a.State), (a, b) =>
            b switch
            {
                ProcessState.Created => RunningState.Play,
                ProcessState.Terminated => RunningState.None,
                ProcessState.Running => RunningState.Stop,
                ProcessState.None => RunningState.None,
                ProcessState.Ready => RunningState.None,
                ProcessState.Blocked => RunningState.None,
                _ => throw new NotImplementedException(),
            })
            .Select(a => new TaskChangeRequest(this.Key, a))
            .Subscribe(requests.OnNext);

            hasPotentialProgress = items.Select(a => a.State).Select(b => b switch
            {
                ProcessState.Created => true,
                ProcessState.Ready => true,
                ProcessState.Running => true,
                _ => false
            }).ToProperty(this, a => a.HasPotentialProgress);

            _ = progressState
                .Transform(a => new ProgressStateSummary(a.Key, a.State, a.Progress.Started))
                .Bind(out progressStatesCollection)
                .Subscribe();

            Command = command;
        }

        public string Key { get; }

        public ProcessState State => _state.Value;

        public ProgressItem Progress => _progress.Value;

        public bool HasPotentialProgress => hasPotentialProgress.Value;

        public ReadOnlyObservableCollection<ProgressStateSummary> Collection => progressStatesCollection;

        public ICommand Command { get; }

        public int CompareTo(ProgressViewModel other)
        {
            return this.Progress.Started.CompareTo(other.Progress.Started);
        }

        public int CompareTo(object obj)
        {
            return this.Progress.Started.CompareTo((obj as ProgressViewModel).Progress.Started);
        }

        public IDisposable Subscribe(IObserver<TaskChangeRequest> observer)
        {
            return requests.Subscribe(observer);
        }
    }
}

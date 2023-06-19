using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Utility.Progressions;

namespace Utility.Progressions.ViewModel
{
    public class KeyProgressViewModel : ReactiveObject, IObserver<IProgressState>
    {
        protected readonly ReplaySubject<IProgressState> progressTasks = new();

        private readonly ObservableAsPropertyHelper<bool> isSavedPropertyHelper;
        private readonly ObservableAsPropertyHelper<bool> isInDeterminateHelper;
        private readonly ObservableAsPropertyHelper<bool> isSaving;
        private readonly ObservableAsPropertyHelper<double> saveProgress;

        public KeyProgressViewModel(string key, IObservable<IProgressState> progress, IScheduler? scheduler = default)
        {
            Key = key;

            if (scheduler != default)
                progress = progress.ObserveOn(scheduler);

            progress.Where(a => a.Key == key).Subscribe(progressTasks.OnNext);
            isSaving = progressTasks.Select(a => !a.Progress.IsComplete).ToProperty(this, a => a.IsInProgress);
            isSavedPropertyHelper = progressTasks.Select(a => a.Progress.IsComplete).ToProperty(this, a => a.IsComplete);
            isInDeterminateHelper = progressTasks.Select(a => a.Progress.IsIndeterminate).ToProperty(this, a => a.IsIndeterminate);
            saveProgress = progressTasks.Select(a => (double)(a.Progress.Value.Decimal * 100m)).ToProperty(this, a => a.Progress);
        }

        public string? Key { get; }

        public bool IsInProgress => isSaving.Value;

        public bool IsIndeterminate => isInDeterminateHelper.Value;

        public bool IsComplete => isSavedPropertyHelper.Value;

        public double Progress => saveProgress.Value;

        public void Dispose()
        {
            isSavedPropertyHelper?.Dispose();
            isSaving?.Dispose();
            saveProgress?.Dispose();
        }

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

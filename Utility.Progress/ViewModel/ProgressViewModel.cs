
using System;

namespace Utility.Progress
{
    public class ProgressViewModel
    { 
        public ProgressViewModel(ProgressItem progressItem)
        {
            IsIndeterminate = progressItem.IsIndeterminate;
            IsComplete = progressItem.IsComplete;
            Progress = progressItem.Value;
            Started = progressItem.Started;
        }

        public bool IsComplete { get; }

        public bool IsIndeterminate { get; }

        public double Progress { get; }
        public DateTime Started { get; }
    }
}
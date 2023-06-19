using System;
using Utility.Structs;

namespace Utility
{
    public struct ProgressItem
    {
        public ProgressItem(Percent value, bool isComplete, bool isIndeterminate, TimeSpan timeRemaining, DateTime started)
        {
            Value = value;
            IsComplete = isComplete;
            IsIndeterminate = isIndeterminate;
            TimeRemaining = timeRemaining;
            Started = started;
            CurrentTime = DateTime.Now;
        }

        public DateTime Started { get; }

        public DateTime CurrentTime { get; }

        public TimeSpan TimeRemaining { get; }

        public bool IsIndeterminate { get; }

        public bool IsComplete { get; }

        public Percent Value { get; }
    }

    //public interface IProgressTask:IObservable<>
    //{
    //    Task<ITaskOutput> Output { get; }

    //    IProgressState Progress { get; }
    //}
}
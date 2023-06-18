using System;
using System.Threading.Tasks;
using Utility.Structs;
using Utility.Tasks.Model;
using UtilityInterface.Generic;

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

    public interface ITaskOutput:IKey<string>
    {
        object Value { get; }
        bool IsCancelled { get; }
    }

    public interface ITaskOutput<T> : IKey<string>
    {
        T Value { get; }
    }

    //public interface IProgressTask:IObservable<>
    //{
    //    Task<ITaskOutput> Output { get; }

    //    IProgressState Progress { get; }
    //}
}
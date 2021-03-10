using UtilityInterface.Generic;
using System;
using System.Collections.Generic;
using System.Text;
using UtilityEnum;
using ReactiveAsyncWorker.Interface;

namespace ReactiveAsyncWorker.Model
{
    public class ProgressState : IEquatable<ProgressState>, IGroupKey<ProcessState>, IKey<string>, IComparable, IComparable<ProgressState>
    {
        public ProgressState(string key, ProcessState state, double progress, bool isIndeterminate, DateTime date = default)
        {
            Key = key;
            State = state;
            Progress = progress;
            IsIndeterminate = isIndeterminate;
            Date = date.Equals(default) ? DateTime.Now : date;
        }

        public string Key { get; }

        public DateTime Date { get; }

        public ProcessState State { get; }

        public double Progress { get; }

        public bool IsIndeterminate { get; }

        public ProcessState GroupKey => State;

        public int CompareTo(object obj)
        {
            return this.CompareTo(obj as ProgressState);
        }

        public int CompareTo(ProgressState other)
        {
            return this.Date.CompareTo(other.Date);
        }

        public override bool Equals(object obj)
        {
            return Equals((ProgressState)obj);
        }

        public bool Equals(ProgressState other)
        {
            return other != null &&
                   Key == other.Key;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Key);
        }

        public static bool operator ==(ProgressState left, ProgressState right)
        {
            return EqualityComparer<ProgressState>.Default.Equals(left, right);
        }

        public static bool operator !=(ProgressState left, ProgressState right)
        {
            return !(left == right);
        }
    }
}

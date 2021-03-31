using System;
using System.Collections.Generic;
using UtilityEnum;

namespace Utility.Tasks.Model
{
    public class ProgressState : IProgressState
    {
        public ProgressState(string key, ProcessState state, ProgressItem progress, ITaskOutput output)
        {
            Key = key;
            State = state;
            Progress = progress;
            Output = output;
        }

        public string Key { get; }

        public ProcessState State { get; }

        public ProgressItem Progress { get; }

        public ITaskOutput? Output { get; }

        ProcessState IGroupKey<ProcessState>.GroupKey => State;

        public int CompareTo(object obj)
        {
            return this.CompareTo(obj as IProgressState);
        }

        public int CompareTo(IProgressState other)
        {
            return this.Progress.Started.CompareTo(other.Progress.Started);
        }

        public override bool Equals(object obj)
        {
            //if (ReferenceEquals(null, obj)) return false;
            //if (ReferenceEquals(this, obj)) return true;
            //if (obj.GetType() != this.GetType()) return false;
            return Equals((IProgressState)obj);
        }

        public bool Equals(IProgressState other)
        {
            return other != null && Key == other.Key && State == other.State;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Key);
        }

        //public bool Equals(ProgressState other)
        //{
        //    return Equals(other as IProgressState);
        //}

        public static bool operator ==(ProgressState left, ProgressState right)
        {
            return EqualityComparer<ProgressState>.Default.Equals(left, right);
        }

        public static bool operator !=(ProgressState left, ProgressState right)
        {
            return !(left == right);
        }

        public static bool operator <(ProgressState left, ProgressState right)
        {
            return left is null ? right is not null : left.CompareTo(right) < 0;
        }

        public static bool operator <=(ProgressState left, ProgressState right)
        {
            return left is null || left.CompareTo(right) <= 0;
        }

        public static bool operator >(ProgressState left, ProgressState right)
        {
            return left is not null && left.CompareTo(right) > 0;
        }

        public static bool operator >=(ProgressState left, ProgressState right)
        {
            return left is null ? right is null : left.CompareTo(right) >= 0;
        }

        public override string ToString()
        {
            return this.Key + " " + DateTime.Now.ToString("t") + " " + this.State;
        }
    }
}

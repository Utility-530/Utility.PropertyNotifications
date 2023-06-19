using System;
using Utility.Enums;
using Utility.Interfaces.Generic;

namespace Utility.Progressions
{
    public interface IProgressState : IEquatable<IProgressState>, IGroupKey<ProcessState>, IKey<string>, IComparable, IComparable<IProgressState>
    {
        ProgressItem Progress { get; }

        ProcessState State { get; }

        ITaskOutput Output { get; }
    }


    public interface IProgressState<T> : IEquatable<IProgressState<T>>, IGroupKey<ProcessState>, IKey<string>, IComparable, IComparable<IProgressState>
    {
        ProgressItem Progress { get; }

        ProcessState State { get; }

        ITaskOutput<T> Output { get; }
    }
}
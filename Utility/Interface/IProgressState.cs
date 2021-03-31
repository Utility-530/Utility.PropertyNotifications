using System;
using UtilityEnum;
using UtilityInterface.Generic;

namespace Utility.Tasks.Model
{
    public interface IProgressState : IEquatable<IProgressState>, IGroupKey<ProcessState>, IKey<string>, IComparable, IComparable<IProgressState>
    {
        ProgressItem Progress { get; }

        ProcessState State { get; }

        ITaskOutput? Output {get;}
    }


    public interface IProgressState<T> : IEquatable<IProgressState<T>>, IGroupKey<ProcessState>, IKey<string>, IComparable, IComparable<IProgressState>
    {
        ProgressItem Progress { get; }

        ProcessState State { get; }

        ITaskOutput<T>? Output { get; }
    }
}
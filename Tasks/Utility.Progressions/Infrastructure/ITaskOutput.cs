using Utility.Interfaces.Generic;

namespace Utility
{

    public interface ITaskOutput : IKey<string>
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
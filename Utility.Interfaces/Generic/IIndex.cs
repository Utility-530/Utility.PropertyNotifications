using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Utility.Interfaces.Generic
{
    public interface IObservableIndex<T>
    {
        System.IObservable<T> this[string key] { get; }
    }
    public interface IIndex<T>
    {
        T this[string key] { get; }
    }

    public interface ITaskIndex<T>
    {
        Task<T> this[string key] { get; }
    }
}

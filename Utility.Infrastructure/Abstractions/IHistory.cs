using System.Collections;
using Utility.Interfaces.NonGeneric;

namespace Utility.Infrastructure.Abstractions

{
    public interface IHistory : IObserver, IObservable
    {
        IEnumerable Past { get; }
        IEnumerable Present { get; }
        IEnumerable Future { get; }
    }
}
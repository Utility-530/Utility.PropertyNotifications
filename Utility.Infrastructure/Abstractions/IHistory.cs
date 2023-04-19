using System.Collections;

namespace Utility.Infrastructure.Abstractions

{
    public interface IHistory : IObserver<object>, IObservable<object>
    {
        IEnumerable Past { get; }
        IEnumerable Present { get; }
        IEnumerable Future { get; }

        void Back();

        void Forward();
    }
}
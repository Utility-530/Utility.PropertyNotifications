using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace NetPrints.Core
{
    public interface IRangeCollection<T>
    {
        void AddRange(IEnumerable<T> range);
        void RemoveRange(IEnumerable<T> range);
    }
    public interface IObservableCollection<T> : INotifyCollectionChanged, IList<T>
    {
    }
}
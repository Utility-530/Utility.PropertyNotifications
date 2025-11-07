using System;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace Utility.Interfaces.NonGeneric
{
    public interface IObservableCollection<T> : IReadOnlyObservableCollection<T>, ICollection<T>
    {
    }

    public interface IReadOnlyObservableCollection<T> : IObservable<NotifyCollectionChangedEventArgs>, INotifyCollectionChanged
    {
    }
}
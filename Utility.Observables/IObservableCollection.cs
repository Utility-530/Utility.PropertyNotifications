using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility.Observables
{
    public interface IObservableCollection<T> : IReadOnlyObservableCollection<T>, ICollection<T>
    {
    }

    public interface IReadOnlyObservableCollection<T> : IObservable<NotifyCollectionChangedEventArgs>, INotifyCollectionChanged
    {
    }
}

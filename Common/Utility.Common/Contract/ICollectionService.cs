using System;
using System.Collections.ObjectModel;
using Utility.Common.Models;

namespace Utility.Common.Contract
{
    public interface ICollectionService : IObserver<RepositoryMessage>, IObservable<CollectionChangeMessage>
    {
        ObservableCollection<object> Items { get; }
    }
}
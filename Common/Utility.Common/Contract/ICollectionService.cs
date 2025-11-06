using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility.Common.Models;

namespace Utility.Common.Contract
{
    public interface ICollectionService : IObserver<RepositoryMessage>, IObservable<CollectionChangeMessage>
    {
        ObservableCollection<object> Items { get; }
    }
}

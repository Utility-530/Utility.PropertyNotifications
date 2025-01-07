using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Threading.Tasks;
using Utility.Interfaces.Generic;
using Utility.Interfaces.NonGeneric;

namespace Utility.Trees.Abstractions
{
    /// <summary>
    /// <a href="https://github.com/yuramag/ObservableTreeDemo"></a>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ITree : IReadOnlyTree, IEquatable<ITree>, IEnumerable<ITree>, IRemove, IRemoveAt, IAdd, INotifyCollectionChanged, INotifyPropertyChanged, ICollection<ITree> //, IRemove<Guid>
    {
        bool HasItems { get; }

        Task<ITree> Add();

        Task<ITree> Remove();

        IIndex Index { get; }

        ITree? this[int index] { get; set; }

        void Remove(Guid index);

        Task<bool> HasMoreChildren();
    }


}
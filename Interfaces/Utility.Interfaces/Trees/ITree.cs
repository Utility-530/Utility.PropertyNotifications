using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Threading.Tasks;
using Utility.Interfaces;
using Utility.Interfaces.NonGeneric;

namespace Utility.Trees.Abstractions
{
    /// <summary>
    /// <a href="https://github.com/yuramag/ObservableTreeDemo"></a>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ITree : IReadOnlyTree, IValue, ISetIsReadOnly, IEquatable<ITree>, IRemove, IRemoveAt, IAdd,
        INotifyCollectionChanged,
        INotifyPropertyChanged,
        INotifyPropertyReceived,
        INotifyPropertyCalled,
        ICollection<ITree> //, IRemove<Guid>
    {
        bool HasChildren { get; }

        ITree? this[int index] { get; set; }

        void Remove(Guid index);

        Task<bool> HasMoreChildren();

        Task<ITree> ToTree(object d);
    }
}
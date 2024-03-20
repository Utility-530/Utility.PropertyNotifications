using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility.Interfaces.Generic;

namespace Utility.Trees.Abstractions
{

    /// <summary>
    /// <a href="https://github.com/yuramag/ObservableTreeDemo"></a>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ITree<T> : ITree, IEnumerable<ITree<T>>, IAdd<ITree<T>>
    {
        new ITree<T>? Parent { get; set; }

        ITree<T> this[T item] { get; set; }
        //new IReadOnlyList<ITree<T>> Items { get; }
        new T Data { get; }

        new ITree<T> this[int index] { get; set; }
        new ITree<T> this[Guid index] { get; set; }

        //void Add(T data);

        void Remove(T data);

        new ITree<T> Add();

        new ITree<T> Remove();

        int IndexOf(ITree<T> tree);
    }
}

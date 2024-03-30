using System;
using System.Collections.Generic;
using Utility.Interfaces.Generic;

namespace Utility.Trees.Abstractions
{

    /// <summary>
    /// <a href="https://github.com/yuramag/ObservableTreeDemo"></a>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ITree<T> : ITree, IEnumerable<ITree<T>>, IAdd<ITree<T>>
    {
        //new ITree<T>? Parent { get; set; }

        //new T Data { get; }

        new ITree<T> this[int index] { get; set; }

        void Remove(T data);

        //new ITree<T> Add();

        //new ITree<T> Remove();

        int IndexOf(ITree<T> tree);
    }
}

using System.Collections;

//

namespace Utility.Trees
{
    /// <summary>
    /// <a href="https://github.com/yuramag/ObservableTreeDemo"></a>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ITree
    {
        bool HasItems { get; }
        void Add(object data);

        IList Items { get; }

        object Data { get; }
    }

    /// <summary>
    /// <a href="https://github.com/yuramag/ObservableTreeDemo"></a>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ITree<T> : ITree, IEnumerable<ITree<T>>
    {
        ITree<T> Parent { get; }
        IEnumerable<ITree<T>> GetParents(bool includingThis);
        IEnumerable<ITree<T>> GetChildren(bool includingThis);

        ITree<T> this[T item] { get; set; }

        T GenericData { get; }

    }
}
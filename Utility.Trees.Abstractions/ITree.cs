using Utility.Trees.Abstractions;

namespace Utility.Trees
{
    /// <summary>
    /// <a href="https://github.com/yuramag/ObservableTreeDemo"></a>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ITree : IEquatable<ITree>, IEnumerable<ITree>
    {
        public Guid Key { get; }
        bool HasItems { get; }

        void Add(object data);

        void Remove(object data);

        Index Index { get; }

        ITree Parent { get; }

        IReadOnlyList<ITree> Items { get; }

        object Data { get; }

        ITree? this[int index] { get; set; }
        ITree? this[Guid index] { get; set; }

        IEnumerable<ITree> GetParents(bool includingThis);

        IEnumerable<ITree> GetChildren(bool includingThis);

        void Remove(Guid index);

        int IndexOf(ITree tree);

        State State { get; set; }
    }

    /// <summary>
    /// <a href="https://github.com/yuramag/ObservableTreeDemo"></a>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ITree<T> : ITree, IEnumerable<ITree<T>>
    {
        new ITree<T> Parent { get; }

        ITree<T> this[T item] { get; set; }
        new IReadOnlyList<ITree<T>> Items { get; }
        new T Data { get; }

        new ITree<T> this[int index] { get; set; }
        new ITree<T> this[Guid index] { get; set; }

        void Add(T data);

        void Remove(T data);

        int IndexOf(ITree<T> tree);
    }
}
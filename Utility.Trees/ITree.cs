using System;
using System.Collections;

//

namespace Utility.Trees
{
    public enum State
    {
        Default, Current, Forward, Back, Up,
    }

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

        ITree Parent { get; }

        IReadOnlyList<ITree> Items { get; }

        object Data { get; }

        ITree this[int index] { get; set; }
        ITree this[Guid index] { get; set; }

        IEnumerable<ITree> GetParents(bool includingThis);
        IEnumerable<ITree> GetChildren(bool includingThis);

        void Remove(Guid index);

        int IndexOf(ITree tree);
        ITree CloneTree();

        State State { get; set; }

    }

    /// <summary>
    /// <a href="https://github.com/yuramag/ObservableTreeDemo"></a>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ITree<T> : ITree, IEnumerable<ITree<T>>
    {
        new ITree<T> Parent { get; }

        IEnumerable<ITree<T>> GetParents(bool includingThis);
        IEnumerable<ITree<T>> GetChildren(bool includingThis);

        ITree<T> this[T item] { get; set; }
        new IReadOnlyList<ITree<T>> Items { get; }
        new T Data { get; }

        new ITree<T> this[int index] { get; set; }
        new ITree<T> this[Guid index] { get; set; }

        void Add(T data);
        void Remove(T data);

        new ITree<T> CloneTree();
        int IndexOf(ITree<T> tree);

    }
}
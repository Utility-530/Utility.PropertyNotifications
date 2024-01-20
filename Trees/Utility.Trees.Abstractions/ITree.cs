using Utility.Interfaces.NonGeneric;

namespace Utility.Trees.Abstractions
{
    /// <summary>
    /// <a href="https://github.com/yuramag/ObservableTreeDemo"></a>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ITree : IReadOnlyTree, IEquatable<ITree>, IEnumerable<ITree>
    {
        bool HasItems { get; }

        void Add(object data);

        void Remove(object data);

        ITree Add();

        ITree Remove();

        Index Index { get; }

        int Depth { get; }

        new ITree Parent { get; set; }


        ITree? this[int index] { get; set; }
        ITree? this[object equatable] { get; set; }

        IEnumerable<ITree> GetParents(bool includingThis);

        IEnumerable<ITree> GetChildren(bool includingThis);

        void Remove(Guid index);

        int IndexOf(ITree tree);

        //new IReadOnlyList<ITree> Items { get; }
        //State State { get; set; }
    }


}
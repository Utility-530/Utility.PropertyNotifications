using Utility.Trees.Abstractions;

namespace Utility.Trees
{
    public interface IDynamicTree : IObservable<ITree>
    {
        IReadOnlyList<ITree> Items { get; }

        State State { get; set; }

        ITree Tree { get; set; }
        ITree Current { get; set; }
    }
}
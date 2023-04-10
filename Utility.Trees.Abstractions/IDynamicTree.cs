namespace Utility.Trees
{
    public interface IDynamicTree : IObservable<ITree>
    {
        IReadOnlyList<ITree> Items { get; }

        State State { get; set; }

        object Data { get; set; }
        ITree Tree { get; set; }
    }
}

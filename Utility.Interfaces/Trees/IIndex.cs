namespace Utility.Trees
{
    public interface IIndex
    {
        int? this[int key] { get; }

        bool IsEmpty { get; }
    }
}
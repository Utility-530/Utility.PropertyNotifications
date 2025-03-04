namespace Utility.Interfaces.NonGeneric
{
    public interface IIsReadOnly
    {
        bool IsReadOnly { get; }
    }

    public interface ISetIsReadOnly
    {
        bool IsReadOnly { set; }
    }
}

namespace Utility.Interfaces.NonGeneric
{
    public interface IGetKey
    {
        string Key { get; }
    }

    public interface ISetKey
    {
        string Key { set; }
    }

    public interface IKey : IGetKey, ISetKey
    {
    }
}
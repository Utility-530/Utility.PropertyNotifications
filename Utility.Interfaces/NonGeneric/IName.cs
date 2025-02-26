namespace Utility.Interfaces.NonGeneric
{
    public interface IName :IGetName, ISetName
    {
    }

    public interface IGetName
    {
        string Name { get; }

    }

    public interface ISetName
    {
        string Name { set; }

    }
}

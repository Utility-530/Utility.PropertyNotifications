namespace Utility.Interfaces.NonGeneric
{
    public interface IName 
    {
        string Name { get; set; }

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

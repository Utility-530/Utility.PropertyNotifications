namespace Utility.Interfaces.NonGeneric
{
    public interface IIsSelected : IGetIsSelected, ISetIsSelected
    {
    }

    public interface IGetIsSelected
    {
        bool IsSelected { get; }
    }

    public interface ISetIsSelected
    {
        bool IsSelected { set; }
    }
}
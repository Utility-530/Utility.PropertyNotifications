namespace Utility.Interfaces.NonGeneric
{
    public interface IReference : IGetReference, ISetReference
    {
    }

    public interface IGetReference
    {
        object Reference { get; }
    }

    public interface ISetReference
    {
        object Reference { set; }
    }
}
namespace Utility.Interfaces.Generic
{

    public interface IParent<T> : IGetParent<T>, ISetParent<T>
    {
    }

    public interface IGetParent<T>
    {
        T Parent { get; }

    }

    public interface ISetParent<T>
    {
        T Parent { set; }

    }

}

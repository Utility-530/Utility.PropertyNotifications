namespace Utility.Interfaces.Generic
{
    public interface IProcess<T>
    {
        void Process(T value);
    }
}
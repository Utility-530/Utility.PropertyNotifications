namespace Utility.Interfaces.Generic
{
    public interface IPredicate<T>
    {
        bool Evaluate(T value);
    }
}
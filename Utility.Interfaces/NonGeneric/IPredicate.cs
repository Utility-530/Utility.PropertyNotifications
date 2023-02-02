namespace Utility.Interfaces.NonGeneric
{
    public interface IPredicate
    {
        bool Invoke(object value);
    }
}
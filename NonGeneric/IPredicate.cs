namespace UtilityInterface.NonGeneric
{
    public interface IPredicate
    {
        bool Invoke(object value);
    }
}
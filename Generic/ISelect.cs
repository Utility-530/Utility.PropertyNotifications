namespace UtilityInterface.Generic.Database
{
    public interface ISelect<T>
    {
        T Select(T item);
    }

}

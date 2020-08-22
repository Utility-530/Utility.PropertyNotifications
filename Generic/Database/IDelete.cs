namespace UtilityInterface.Generic.Database
{
    public interface IDelete<T>
    {
        bool Delete(T item);
    }

}

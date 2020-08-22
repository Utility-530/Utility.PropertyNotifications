namespace UtilityInterface.Generic.Database
{
    public interface IInsert<T>
    {
        bool Insert(T item);
    }

}

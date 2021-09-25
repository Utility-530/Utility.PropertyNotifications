namespace UtilityInterface.Generic.Database
{
    public interface IId<T>
    {
        T Id { get; }
    }
    public interface IIdSet<T>
    {
        T Id { set; }
    }
}


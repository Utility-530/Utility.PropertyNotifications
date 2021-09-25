namespace UtilityInterface.Generic.Data
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


namespace Utility.Interfaces.NonGeneric.Data
{
    public interface IId
    {
        long Id { get; }
    }

    public interface IIdSet
    {
        long Id { set; }
    }
}
namespace Utility.Interfaces.NonGeneric.Data
{
    public interface IChild : IId
    {
        long ParentId { get; }
    }
}
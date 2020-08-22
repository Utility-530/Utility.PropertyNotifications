using UtilityInterface.NonGeneric.Database;

namespace UtilityInterface.Generic.Database
{
    public interface IChildRow<T> : IChildRow where T : IId
    {
        T Parent { get; set; }
    }

}

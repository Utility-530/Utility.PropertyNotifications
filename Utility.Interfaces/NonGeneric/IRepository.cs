using System.Threading.Tasks;

namespace Utility.Interfaces.NonGeneric
{
    public interface IRepository
    {
        IEquatable Key { get; }

        Task<object?> FindValue(IEquatable key);

        Task<IEquatable[]> FindKeys(IEquatable key);

        Task Update(IEquatable key, object value);
    }
}
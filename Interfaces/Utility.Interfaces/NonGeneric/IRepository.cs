using System.Threading.Tasks;
using Utility.Interfaces.NonGeneric;

namespace Utility.Infrastructure.Abstractions
{
    public interface IRepository
    {
        IEquatable Key { get; }

        Task<object?> FindValue(IEquatable key);

        Task<IEquatable[]> FindKeys(IEquatable key);

        Task Update(IEquatable key, object value);
    }
}
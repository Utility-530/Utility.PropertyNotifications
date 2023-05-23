using System.Threading.Tasks;
using Utility.Interfaces.NonGeneric;

namespace Utility.Infrastructure.Abstractions
{
    public interface IRepository
    {
        Task<object?> FindValue(IEquatable key);

        Task<IEquatable> FindKeyByParent(IEquatable key);

        Task UpdateValue(IEquatable key, object value);
    }
}
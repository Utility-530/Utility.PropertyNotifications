using Abstractions;

namespace PropertyTrees.Infrastructure
{
    public interface IRepository
    {
        Task<object> FindValue(IKey key);

        Task<IKey> FindKeyByParent(IKey key);

        Task UpdateValue(IKey key, object value);
    }
}
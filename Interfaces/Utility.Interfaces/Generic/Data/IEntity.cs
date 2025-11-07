using Utility.Interfaces.NonGeneric.Data;

namespace Utility.Interfaces.Generic.Data
{
    public interface IEntity<T> : IEntity
    {
        T Object { get; set; }
    }
}
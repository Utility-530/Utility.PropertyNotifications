using Utility.Interfaces.NonGeneric.Data;

namespace Utility.Interfaces.Generic.Data
{
    public interface IChild<T> : IChild where T : IId
    {
        T Parent { get; }
    }
}
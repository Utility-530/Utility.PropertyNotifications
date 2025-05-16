using NetPrints.Graph;
using NetPrints.Interfaces;

namespace NetPrints.Core
{
    public interface IMethodParameter : IName
    {
        IBaseType Value { get; }
        bool HasExplicitDefaultValue { get; }
        object ExplicitDefaultValue { get; }
    }
}
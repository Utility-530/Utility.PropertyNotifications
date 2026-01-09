using NetPrints.Interfaces;

namespace NetPrints.Core
{
    public interface IConstructorSpecifier
    {
        ITypeSpecifier DeclaringType { get; }
    }
}
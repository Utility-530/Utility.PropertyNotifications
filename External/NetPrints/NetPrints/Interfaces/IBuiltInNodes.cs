using NetPrints.Interfaces;

namespace NetPrints.WPF.Demo
{
    public interface IBuiltInNodes
    {
        ITypeSpecifier[] this[GraphType graph] { get; }
    }
}
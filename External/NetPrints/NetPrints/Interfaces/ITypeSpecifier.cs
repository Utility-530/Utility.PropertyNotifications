using NetPrints.Core;
using NetPrints.Graph;

namespace NetPrints.Interfaces
{
    public interface ITypeSpecifier : ISpecifier, IBaseType, IFullCodeName
    {
        IObservableCollection<IBaseType> GenericArguments { get; }
    }
}
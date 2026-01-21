using NetPrints.Core;
using System.Collections.Generic;

namespace NetPrints.Interfaces
{
    public interface IReflectionProvider
    {
        IEnumerable<ITypeSpecifier> GetTypes();
        IEnumerable<IMethodSpecifier> GetMethods(IReflectionProviderMethodQuery? query);
        IEnumerable<IVariableSpecifier> GetVariables(IReflectionProviderVariableQuery query);
    }
}

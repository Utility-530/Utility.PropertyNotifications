using NetPrints.Core;
using System.Collections.Generic;

namespace NetPrints.Interfaces
{
    /// <summary>
    /// Interface for reflecting on types, methods etc.
    /// </summary>
    public interface IReflectionProvider
    {
        //bool TypeSpecifierIsSubclassOf(ITypeSpecifier a, ITypeSpecifier b);
        //bool HasImplicitCast(ITypeSpecifier fromType, ITypeSpecifier toType);

        //GetNonStaticTypes
        IEnumerable<ITypeSpecifier> GetTypes();

        //IEnumerable<IMethodSpecifier> GetOverridableMethodsForType(ITypeSpecifier typeSpecifier);
        //IEnumerable<IMethodSpecifier> GetPublicMethodOverloads(IMethodSpecifier methodSpecifier);
        //IEnumerable<IConstructorSpecifier> GetConstructors(ITypeSpecifier typeSpecifier);
        //IEnumerable<string> GetEnumNames(ITypeSpecifier typeSpecifier);

        IEnumerable<IMethodSpecifier> GetMethods(IReflectionProviderMethodQuery? query);
        IEnumerable<IVariableSpecifier> GetVariables(IReflectionProviderVariableQuery query);

        // Documentation
        //string GetMethodDocumentation(IMethodSpecifier methodSpecifier);
        //string GetMethodParameterDocumentation(IMethodSpecifier methodSpecifier, int parameterIndex);
        //string GetMethodReturnDocumentation(IMethodSpecifier methodSpecifier, int returnIndex);
    }
}

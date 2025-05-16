using NetPrints.Enums;
using NetPrints.Graph;
using NetPrints.Interfaces;
using System.Collections.Generic;

namespace NetPrints.Core
{
    public interface IMethodSpecifier : ISpecifier, IName, IDeclaringType, IModifiers, IArgumentTypes
    {
        IList<IBaseType> ReturnTypes { get; }
        IList<IMethodParameter> Parameters { get; }
        MemberVisibility Visibility { get; }
        IList<IBaseType> GenericArguments { get; }
    }

    public interface IDeclaringType
    {
        ITypeSpecifier DeclaringType { get; }
    } 
    
    public interface IArgumentTypes
    {
        //ITypeSpecifier DeclaringType { get; }
        IReadOnlyList<IBaseType> ArgumentTypes { get; }

    }

    public interface IModifiers
    {
        MethodModifiers Modifiers { get; }
    }
}
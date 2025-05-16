using System;
using System.Text;

namespace NetPrints.Interfaces
{
    public interface IType
    {
        ITypeSpecifier Type { get; set; }
    }

    public interface IStatic
    {
        bool? Static { get; set; }
    }


    public interface IReflectionProviderQuery: IType, IStatic
    {
        ITypeSpecifier VisibleFrom { get; set; }

    }

    public interface IReflectionProviderVariableQuery : IReflectionProviderQuery
    {
        ITypeSpecifier VariableType { get; set; }
        bool VariableTypeDerivesFrom { get; set; }
    }

    public interface IReflectionProviderMethodQuery : IReflectionProviderQuery
    {
        ITypeSpecifier ArgumentType { get; set; }
        bool? HasGenericArguments { get; set; }
        ITypeSpecifier ReturnType { get; set; }
 
    }


}

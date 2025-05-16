using NetPrints.Enums;
using System;

namespace NetPrints.Interfaces
{
    public interface IVariable : IName
    {
        MemberVisibility Visibility { get; }
        VariableModifiers Modifiers { get; }
        bool HasAccessors { get; }
        ITypeSpecifier Type { get; }
        IVisibility GetterMethod { get; }
        IVisibility SetterMethod { get; }
    }

    public interface IVisibility
    {
        MemberVisibility Visibility { get; }
    }
}
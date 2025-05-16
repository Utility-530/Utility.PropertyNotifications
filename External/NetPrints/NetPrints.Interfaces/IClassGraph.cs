using NetPrints.Core;
using NetPrints.Enums;
using NetPrints.Graph;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetPrints.Interfaces
{
    public interface IClassGraph : INodeGraph, IVisibility, IFullName, IName, INamespace
    {
        IObservableCollection<IMethodGraph> Methods { get; }
        IObservableCollection<IVariable> Variables { get; }
        IObservableCollection<IConstructorGraph> Constructors { get; }

        ClassModifiers Modifiers { get; }
        IProject Project { get; set; }

        IObservableCollection<IGenericType> DeclaredGenericArguments { get; }
        IEnumerable<ITypeSpecifier> AllBaseTypes { get; }
        ITypeSpecifier Type { get; }
    }
}

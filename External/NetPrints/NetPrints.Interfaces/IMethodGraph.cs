using NetPrints.Core;
using NetPrints.Enums;
using NetPrints.Graph;
using System.Collections;
using System.Collections.Generic;

namespace NetPrints.Interfaces
{
    public interface IMethodGraph : IExecutionGraph, IName
    {
        IReturnNode MainReturnNode { get; }
        MethodModifiers Modifiers { get; }
        IEnumerable<IBaseType> ReturnTypes { get; }
        IEnumerable<IGenericType> GenericArgumentTypes { get; }
        IEnumerable<IReturnNode> ReturnNodes { get; }
        IMethodEntryNode MethodEntryNode { get; }
    }
}

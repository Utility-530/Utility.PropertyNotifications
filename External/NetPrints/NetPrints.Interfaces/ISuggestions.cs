using NetPrints.Graph;
using NetPrints.Interfaces;
using NetPrints.Reflection;
using System.Collections.Generic;

namespace NetPrints.WPF.Demo
{
    public interface ISuggestions
    {
        IEnumerable<ITypesProvider> Get(NodeDataPinType type, GraphType? graphType = null, ITypeSpecifier? graphClassType = null, IEnumerable<ITypeSpecifier>? graphClassBaseTypes = null, ITypeSpecifier? otherType = null);
    }
}
using NetPrints.Core;
using System.Collections.Generic;

namespace NetPrints.Reflection
{
    public interface ITypesProvider
    {
        string Name { get; }

        IEnumerable<ISpecifier> types();
    }
}
using NetPrints.Core;
using NetPrints.Reflection;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetPrints.Interfaces
{
    public interface ITypesEnumerable
    {
        IEnumerable<ITypesProvider> Types { get; }
        //IEnumerable<ISpecifier> Specifiers { get; }
    }
}

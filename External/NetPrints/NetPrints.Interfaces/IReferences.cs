using NetPrints.Core;
using System.Collections.Generic;

namespace NetPrints.WPF.Demo
{
    public interface IReferences
    {
        IEnumerable<IAssemblyReference> References { get; }
    }
}
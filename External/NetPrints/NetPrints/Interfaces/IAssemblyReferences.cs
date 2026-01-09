using NetPrints.Core;
using System.Collections.Generic;

namespace NetPrints.WPF.Demo
{
    public interface IAssemblyReferences
    {
        IEnumerable<IAssemblyReference> References { get; }
    }
}
using System.Collections.Generic;
using System.Reflection;

namespace Utility.Interfaces.Exs
{
    public interface INodeMethodFactory
    {
        IEnumerable<MethodInfo> Methods { get; }
    }

}

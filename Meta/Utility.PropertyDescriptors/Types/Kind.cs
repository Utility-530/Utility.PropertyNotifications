using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility.Descriptors.Types
{
    public enum Kind
    {
        Property,
        Method,

    }

    public interface IKind
    {
        Kind Kind { get; }
    }
}

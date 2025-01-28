using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Utility.Interfaces.NonGeneric
{
    public interface IContext
    {
        SynchronizationContext UI { get; }
    }
}

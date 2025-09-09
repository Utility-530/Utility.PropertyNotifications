using System;
using Utility.Interfaces.Generic;
using Utility.Interfaces.Generic.Data;
using Utility.Interfaces.NonGeneric;

namespace Utility.Nodify.Core
{
    public interface IConnectionViewModel : IKey, IGuid
    {
        IConnectorViewModel Input { get; }
        IConnectorViewModel Output { get; }
    }
}
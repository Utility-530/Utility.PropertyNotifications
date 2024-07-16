using System;

namespace Utility.Nodify.Core
{
    public interface IConnectionViewModel
    {
        Guid Id { get; }

        NodeState State { get; set; }
        IConnectorViewModel Input { get; }
        IConnectorViewModel Output { get; }
        Key Key { get; }
    }
}
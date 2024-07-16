using System;
using System.Windows;

namespace Utility.Nodify.Core
{
    public interface INodeViewModel
    {
        Guid Id { get; }
        ICollection<IConnectorViewModel> Input { get; }
        ICollection<IConnectorViewModel> Output { get; }
        string Title { get; set; }
        Point Location { get; set; }
        NodeState State { get; set; }
        Key Key { get; }
        ICore Core { get; set; }
    }
}
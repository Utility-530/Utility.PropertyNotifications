using System.Drawing;
using Utility.Enums;
using Utility.Interfaces.NonGeneric;

namespace Utility.Nodify.Core
{
    public interface INodeViewModel: IKey, IGuid, IData
    {
        ICollection<IConnectorViewModel> Input { get; }
        ICollection<IConnectorViewModel> Output { get; }
        PointF Location { get; set; }
        NodeState State { get; set; }
        IDiagramViewModel Graph { get; set; }
        Orientation Orientation { get; }
    }
}
using Utility.Interfaces.NonGeneric;

namespace Utility.Infrastructure
{
    public interface IGraphController : IObserver
    {
        Outputs[] Connections { get; set; }
        object Nodes { get; set; }
    }
}

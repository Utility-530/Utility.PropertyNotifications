using Utility.Interfaces.NonGeneric;

namespace Utility.Interfaces.Exs.Diagrams
{
    public interface IConnectionViewModel : IKey, IGuid
    {
        IConnectorViewModel Input { get; }
        IConnectorViewModel Output { get; }
    }
}
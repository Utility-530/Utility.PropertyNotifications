using Utility.Interfaces.Exs.Diagrams;

namespace Utility.Nodify.Core
{
    public record ConnectionMessage(string Key, IConnectionViewModel Connection) : Message(Key, default);

}

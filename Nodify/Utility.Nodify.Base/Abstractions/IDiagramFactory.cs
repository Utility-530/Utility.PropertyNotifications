using Utility.Interfaces.Exs.Diagrams;

namespace Utility.Nodify.Base.Abstractions
{
    public interface IDiagramFactory
    {
        Task Build(IDiagramViewModel diagram);
    }
}
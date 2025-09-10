using Utility.Nodify.Core;

namespace Utility.Nodify.Base.Abstractions
{
    public interface IDiagramFactory
    {
        Task Build(IDiagramViewModel diagram);
    }
}
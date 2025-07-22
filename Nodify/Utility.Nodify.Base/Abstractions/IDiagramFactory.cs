using Utility.Nodify.Core;

namespace Utility.Nodify.Engine
{
    public interface IDiagramFactory
    {
        void Build(IDiagramViewModel diagram);
    }
}
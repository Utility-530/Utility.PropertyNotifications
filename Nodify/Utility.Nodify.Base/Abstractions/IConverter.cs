using Utility.Nodify.Core;
using Utility.Nodify.Entities;

namespace Utility.Nodify.Engine.Infrastructure
{
    public interface IConverter
    {
        IDiagramViewModel Convert(Diagram diagram);
        INodeViewModel Convert(Node node);
        Diagram ConvertBack(IDiagramViewModel diagramViewModel);

    }
}
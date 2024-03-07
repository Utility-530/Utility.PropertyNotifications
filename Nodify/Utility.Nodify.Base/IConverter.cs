using Utility.Nodify.Core;

namespace Utility.Nodify.Engine.Infrastructure
{
    public interface IConverter
    {
        IDiagramViewModel Convert(Diagram diagram);
        INodeViewModel Convert(Node node);
        Diagram ConvertBack(IDiagramViewModel diagramViewModel);

    }
}
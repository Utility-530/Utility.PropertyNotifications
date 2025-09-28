using Utility.Interfaces.Exs.Diagrams;
using Utility.Nodify.Core;
using Utility.Nodify.Entities;

namespace Utility.Nodify.Base.Abstractions
{
    public interface IConverter
    {
        IDiagramViewModel Convert(Diagram diagram);
        INodeViewModel Convert(Node node);
        Diagram ConvertBack(IDiagramViewModel diagramViewModel);

    }
}
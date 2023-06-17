using System.Windows.Controls;
using Utility.Models;
using Utility.Nodes;

namespace Utility.PropertyTrees.WPF.Meta
{
    public record TreeViewRequest(TreeView TreeView, ValueNode PropertyNode) : Request;
    public record TreeViewResponse(TreeView TreeView) : Response(TreeView);
    public record TreeClickEvent(ValueNode ValueNode) : Event;
    public record TreeMouseDoubleClickEvent(ValueNode ValueNode) : Event;

}

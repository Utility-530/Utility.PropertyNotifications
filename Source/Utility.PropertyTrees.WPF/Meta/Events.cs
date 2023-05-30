using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using Utility.Models;
using Utility.Nodes;

namespace Utility.PropertyTrees.WPF.Meta
{
    public record TreeViewRequest(TreeView TreeView, ValueNode PropertyNode) : Request;
    public record TreeViewResponse(TreeView TreeView) : Response(TreeView);

}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Utility.Trees.Abstractions;
using Utility.Interfaces.NonGeneric;

namespace Utility.Nodes.Demo
{
    public class StyleSelector : System.Windows.Controls.StyleSelector
    {
        public override Style SelectStyle(object item, DependencyObject container)
        {
            if(item is IReadOnlyTree { Data: IDescriptor data })
            {
                return Utility.Nodes.WPF.StyleSelector.Instance.SelectStyle(item, container);
            }

            return base.SelectStyle(item, container);
        }
    }
}

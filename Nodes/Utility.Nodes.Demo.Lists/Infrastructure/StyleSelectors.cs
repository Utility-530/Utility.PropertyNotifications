using System.Windows.Controls;
using System.Windows;
using Utility.WPF.Helpers;
using Utility.Models.Trees;
using Utility.Models;
using StyleSelector = System.Windows.Controls.StyleSelector;

namespace Utility.Nodes.Demo.Lists
{
    public class ContainerStyleSelector : StyleSelector
    {
        public override Style SelectStyle(object item, DependencyObject container)
        {
            var parent = (container as TreeViewItem).FindParent<TreeViewItem>();

            return item switch
            {
                _ => DefaultStyle ?? base.SelectStyle(item, container),
            };
        }

        public Style DefaultStyle { get; set; }

    }
}
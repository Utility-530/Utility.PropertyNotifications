using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;
using Utility.Interfaces.NonGeneric;
using Utility.WPF.Helpers;
using Utility.Models.Trees;

namespace Utility.Nodes.Demo.Filters
{
    public class ComboContainerStyleSelector : StyleSelector
    {

        public override Style SelectStyle(object item, DependencyObject container)
        {
            var parent = (container as TreeViewItem).FindParent<TreeViewItem>();

            return item switch
            {
                IData { Data: ISelection } => ComboStyle,
                //IData { Data: IRoot } => BreadcrumbRootStyle,
                //IData { Data: IBreadCrumb } when parent?.Style == BreadcrumbStyle => SelectableStyle,
                //IData { Data: IBreadCrumb } => BreadcrumbStyle,
                _ => DefaultStyle ?? base.SelectStyle(item, container),
            };
        }

        public Style DefaultStyle { get; set; }
        public Style ComboStyle { get; set; }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using Utility.Interfaces.NonGeneric;
using Utility.WPF.Helpers;

namespace Utility.WPF.Controls.Breadcrumbs
{
    public class ItemContainerStyleSelector : StyleSelector
    {

        public override Style SelectStyle(object item, DependencyObject container)
        {
            var parent = (container as TreeViewItem).FindParent<TreeViewItem>();

            return item switch
            {
                IData { Data: IRoot } => BreadcrumbRootStyle,
                IData { Data: IBreadCrumb } when parent?.Style == BreadcrumbStyle => SelectableStyle,
                IData { Data: IBreadCrumb } => BreadcrumbStyle,
                _ => DefaultStyle ?? base.SelectStyle(item, container),
            };
        }

        public Style DefaultStyle { get; set; }
        public Style BreadcrumbStyle { get; set; }
        public Style SelectableStyle { get; set; }
        public Style BreadcrumbRootStyle { get; set; }
    }
}


using System.Windows.Controls;
using System.Windows;
using Utility.Interfaces.NonGeneric;
using Utility.WPF.Helpers;
using Utility.Models.Trees;
using Utility.Models;
using StyleSelector = System.Windows.Controls.StyleSelector;

namespace Utility.Nodes.Demo.Lists
{
    public class ComboContainerStyleSelector : StyleSelector
    {

        public override Style SelectStyle(object item, DependencyObject container)
        {
            var parent = (container as TreeViewItem).FindParent<TreeViewItem>();

            return item switch
            {
                 ISelectable => ComboStyle,
                //IData { Data: IRoot } => BreadcrumbRootStyle,
                //IData { Data: IBreadCrumb } when parent?.Style == BreadcrumbStyle => SelectableStyle,
                //IData { Data: IBreadCrumb } => BreadcrumbStyle,
                _ => DefaultStyle ?? base.SelectStyle(item, container),
            };
        }

        public Style DefaultStyle { get; set; }
        public Style ComboStyle { get; set; }

    }  
    
    
    public class CollectionStyleSelector : StyleSelector
    {

        public override Style SelectStyle(object item, DependencyObject container)
        {
            var parent = (container as TreeViewItem).FindParent<TreeViewItem>();

            return item switch
            {
                 IChildCollection => CollectionStyle,
                 ICollectionItem => ItemStyle,
            };
        }

        public Style CollectionStyle { get; set; }
        public Style ItemStyle { get; set; }

    }
}

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
using Utility.Models;

namespace Utility.Nodes.Demo.Transformers
{
    public class ComboContainerStyleSelector : StyleSelector
    {

        public override Style SelectStyle(object item, DependencyObject container)
        {
             var parent = (container as TreeViewItem).FindParent<TreeViewItem>();

            return item switch
            {
                IData { Data: ISelectable } => ComboStyle,
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
                IData { Data: IChildCollection } => CollectionStyle,
                IData { Data: ICollectionItem } => ItemStyle,
            };
        }

        public Style CollectionStyle { get; set; }
        public Style ItemStyle { get; set; }

    }
}

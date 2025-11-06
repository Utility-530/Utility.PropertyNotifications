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
using StyleSelector = System.Windows.Controls.StyleSelector;
using DataTemplateSelector = System.Windows.Controls.DataTemplateSelector;
using Utility.PropertyDescriptors;

namespace Utility.Nodes.Demo.Editor
{
    public class ContainerStyleSelector : StyleSelector
    {

        public override Style SelectStyle(object item, DependencyObject container)
        {
            //var parent = (container as TreeViewItem).FindParent<TreeViewItem>();

            return item switch
            {
                DataFilesModel => ComboStyle,
                DataFileModel => DefaultStyle,
                MemberDescriptor => Utility.Nodes.WPF.StyleSelector.Instance.SelectStyle(item, container),
                _ => base.SelectStyle(item, container),
            };
        }

        public Style DefaultStyle { get; set; }
        public Style ComboStyle { get; set; }
        //public Style SlaveStyle { get; set; }
        //public StyleSelector StyleSelector { get; set; }

    }

    public class ContainerTemplateSelector : DataTemplateSelector
    {
        Models.Templates.ModelTemplateSelector ModelTemplateSelector = new();
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            return item switch
            {
                Model => ModelTemplateSelector.SelectTemplate(item, container),
                MemberDescriptor => Utility.WPF.Trees.Filters.DataTemplateSelector.Instance.SelectTemplate(item, container),
                ProliferationModel => ModelTemplateSelector.SelectTemplate(item, container),
                _ => throw new Exception("DVS")
            };
        }
    }

    //public class CollectionStyleSelector : StyleSelector
    //{

    //    public override Style SelectStyle(object item, DependencyObject container)
    //    {
    //        var parent = (container as TreeViewItem).FindParent<TreeViewItem>();

    //        return item switch
    //        {
    //            //IChildCollection  => CollectionStyle,
    //            _ => ItemStyle,
    //        };
    //    }

    //    public Style CollectionStyle { get; set; }
    //    public Style ItemStyle { get; set; }

    //}
}

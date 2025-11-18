using System.Windows;
using Utility.Models;
using Utility.Models.Templates;
using Utility.Models.Trees;
using Utility.PropertyDescriptors;
using DataTemplateSelector = System.Windows.Controls.DataTemplateSelector;
using StyleSelector = System.Windows.Controls.StyleSelector;

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
                NodeViewModel { Name: NodeMethodFactory.Slave } => TableStyle,
                NodeViewModel { DataTemplate: "TabStyle" } => TabStyle,
                DataFileModel => DefaultStyle,
                MemberDescriptor => Utility.Nodes.WPF.StyleSelector.Instance.SelectStyle(item, container),
                Model => DefaultStyle,
                _ => base.SelectStyle(item, container),
            };
        }

        public Style DefaultStyle { get; set; }
        public Style ComboStyle { get; set; }
        public Style TableStyle { get; set; }
        public Style TabStyle { get; set; }

    }

    public class SelectedItemStyleSelector : StyleSelector
    {
        public override Style SelectStyle(object item, DependencyObject container)
        {
            //var parent = (container as TreeViewItem).FindParent<TreeViewItem>();

            return item switch
            {
                NodeViewModel { Name: "MasterTab" } => DragablzItemStyle,
                _ => base.SelectStyle(item, container),
            };
        }

        public Style DragablzItemStyle { get; set; }
    }

    public class ContainerTemplateSelector : DataTemplateSelector
    {
        Models.Templates.ModelTemplateSelector ModelTemplateSelector = new();
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            return item switch
            {
                MemberDescriptor => Utility.WPF.Trees.Filters.DataTemplateSelector.Instance.SelectTemplate(item, container),
                NodeViewModel { DataTemplate: "TabStyle" } => TabTemplate,            
                Model => ModelTemplateSelector.SelectTemplate(item, container),       
                _ => throw new Exception("DVS")
            };
        }

        public DataTemplate TabTemplate { get;set; }
    }
    public class SelectedItemTemplateSelector : DataTemplateSelector
    {
        Models.Templates.SelectedItemTemplateSelector ModelTemplateSelector = new();
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            return item switch
            {
                null => ModelTemplateSelector.SelectTemplate(item, container),
                Model => ModelTemplateSelector.SelectTemplate(item, container),
                MemberDescriptor => Utility.WPF.Trees.Filters.DataTemplateSelector.Instance.SelectTemplate(item, container),
                ProliferationModel => ModelTemplateSelector.SelectTemplate(item, container),
                _ => throw new Exception("DVS")
            };
        }
    }
}

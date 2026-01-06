using System.Windows;
using Utility.Interfaces.NonGeneric;
using Utility.Models;
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
                NodeViewModel { Name: NodeMethodFactory.Files } => ComboStyle,
                NodeViewModel { Name: NodeMethodFactory.Slave } => TableStyle,
                NodeViewModel { Parent: IGetName { Name: NodeMethodFactory.Slave } } => TabStyle,
                MemberDescriptor => Utility.WPF.Trees.StyleSelector.Instance.SelectStyle(item, container),
                Model => DefaultStyle,
                _ => base.SelectStyle(item, container),
            };
        }

        public Style DefaultStyle { get; set; }
        public Style ComboStyle { get; set; }
        public Style TableStyle { get; set; }
        public Style TabStyle { get; set; }
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
                MemberDescriptor => Utility.WPF.Trees.DataTemplateSelector.Instance.SelectTemplate(item, container),
                _ => throw new Exception("DVS")
            };
        }
    }
}

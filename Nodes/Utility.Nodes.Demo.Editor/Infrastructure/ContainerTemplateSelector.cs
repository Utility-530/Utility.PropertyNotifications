using System.Windows;
using System.Windows.Automation.Text;
using Utility.Interfaces.NonGeneric;
using Utility.Models;
using Utility.Models.Templates;
using Utility.PropertyDescriptors;
using DataTemplateSelector = System.Windows.Controls.DataTemplateSelector;

namespace Utility.Nodes.Demo.Editor
{
    public class ContainerTemplateSelector : DataTemplateSelector
    {
        Models.Templates.ModelTemplateSelector ModelTemplateSelector = new();
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            return item switch
            {
                NodeViewModel { Name: NodeMethodFactory.Slave } => TableTemplate,
                NodeViewModel { Parent: IGetName { Name: NodeMethodFactory.Slave } } => TabTemplate,
                MemberDescriptor => Utility.WPF.Trees.DataTemplateSelector.Instance.SelectTemplate(item, container),
                //NodeViewModel { DataTemplate: "TabStyle" } => TabTemplate,            
                Model => ModelTemplateSelector.SelectTemplate(item, container),       
                _ => throw new Exception("DVS")
            };
        }

        public DataTemplate TableTemplate { get;set; }
        public DataTemplate TabTemplate { get;set; }
    }
}

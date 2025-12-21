using System.Windows;
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
                MemberDescriptor => Utility.WPF.Trees.DataTemplateSelector.Instance.SelectTemplate(item, container),
                //NodeViewModel { DataTemplate: "TabStyle" } => TabTemplate,            
                Model => ModelTemplateSelector.SelectTemplate(item, container),       
                _ => throw new Exception("DVS")
            };
        }

        public DataTemplate TabTemplate { get;set; }
    }
}

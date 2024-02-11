using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Utility.PropertyDescriptors;
using Utility.Trees.Abstractions;
using Utility.WPF.Nodes;
using Utility.WPF.Nodes.NewFolder;

namespace Utility.Nodes.Reflections.Demo.Infrastructure
{
    public class CustomStyleSelector : StyleSelector
    {
        public override Style SelectStyle(object item, DependencyObject container)
        {

            //var style = NewTemplates["CustomTreeViewItemStyle"] as Style;
            //return style;
            if (item is TreeViewItem{ Header: IReadOnlyTree { Data: ICollectionItemDescriptor { } } })
            {
                var style = NewTemplates["CustomTreeViewItemStyle"] as Style;
                return style;
            }

            return TreeViewItemStyleSelector.Instance.SelectStyle(item, container);
        }


        public ResourceDictionary NewTemplates
        {
            get
            {
                var resourceDictionary = new ResourceDictionary
                {
                    Source = new Uri($"/{typeof(CustomStyleSelector).Assembly.GetName().Name};component/Themes/Generic.xaml", UriKind.RelativeOrAbsolute)
                };
                return resourceDictionary;
            }
        }

        public static CustomStyleSelector Instance { get; } = new();

    }
}

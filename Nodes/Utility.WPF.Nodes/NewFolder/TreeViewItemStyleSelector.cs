using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Utility.WPF.Nodes.NewFolder
{
    public class TreeViewItemStyleSelector:StyleSelector
    {
        public override Style SelectStyle(object item, DependencyObject container)
        {
            var style = Templates["CustomTreeViewItemStyle"] as Style;
            return style;
            //return base.SelectStyle(item, container);
        }


        public ResourceDictionary Templates
        {
            get
            {
                var resourceDictionary = new ResourceDictionary
                {
                    Source = new Uri($"/{typeof(ITreeViewBuilder).Assembly.GetName().Name};component/Themes/Generic.xaml", UriKind.RelativeOrAbsolute)
                };
                return resourceDictionary;
            }
        }

        public static TreeViewItemStyleSelector Instance { get; } = new();
    }
}

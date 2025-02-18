using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;
using Utility.Trees.Abstractions;
using Utility.Interfaces.NonGeneric;

namespace Utility.Nodes.Demo
{
    public class DataTemplateSelector : System.Windows.Controls.DataTemplateSelector
    {
        public DataTemplate? DefaultTemplate { get; set; }

        public override DataTemplate? SelectTemplate(object item, DependencyObject container)
        {
            if(item is IReadOnlyTree { Data: IDescriptor data } )
            {
                return Utility.Nodes.WPF.DataTemplateSelector.Instance.SelectTemplate(item, container);
            }
            return DefaultTemplate;
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Utility.Interfaces.NonGeneric;

namespace Utility.Nodes.WPF
{
    internal class CustomTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is IGetName { Name: ChildrenSelector.IsCollection })
            {
                return Boolean;
            }
            return Default;
        }

        public DataTemplate Default { get; set; }
        public DataTemplate Boolean { get; set; }
    }
}

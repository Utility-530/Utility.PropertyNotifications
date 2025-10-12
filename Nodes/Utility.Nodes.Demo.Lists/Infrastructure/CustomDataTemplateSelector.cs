using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Utility.Interfaces.NonGeneric;

namespace Utility.Nodes.Demo.Lists
{
    internal class CustomDataTemplateSelector: DataTemplateSelector
    {

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if(item is IGetName { Name: nameof(Factories.NodeMethodFactory.BuildListRoot)})
            {
                return MasterTemplate ?? DefaultTemplate;
            }
            return DefaultTemplate;
        }
        public DataTemplate DefaultTemplate { get; set; }
        public DataTemplate MasterTemplate { get; set; }



    }
}

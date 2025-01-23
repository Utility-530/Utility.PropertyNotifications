using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Utility.Models.Trees;

namespace Utility.Nodes.Demo.Filters
{
    public class EditTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            switch(item)
            {
                case ResolvableModel:
                    return ResolvableModelTemplate;     
                case AndOrModel:
                    return AndOrModelTemplate;
            }

            return base.SelectTemplate(item, container);
        }

        public DataTemplate ResolvableModelTemplate { get; set; }
        public DataTemplate AndOrModelTemplate { get; set; }
    }
}

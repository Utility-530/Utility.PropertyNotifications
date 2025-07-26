using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Utility.Nodify.ViewModels;

namespace Utility.Nodify.Transitions.Demo
{
    public class TemplateSelector:DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var template = base.SelectTemplate(item, container);
            if (template == null)
            {
                var x = Application.Current.Resources[new DataTemplateKey(item.GetType())] as DataTemplate;
                return x ?? throw new Exception("33 sfdf4");
            }
            return template;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Utility.Models.Templates;
using Utility.Nodify.Views.Infrastructure;

namespace Utility.Nodify.Demo.Infrastructure
{
    public class TemplateSelector : DataTemplateSelector
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


    internal class ContainerTemplateSelector : DataTemplateSelector
    {
        ModelTemplateSelector modelTemplateSelector = new ModelTemplateSelector();
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            //return Application.Current.Resources["EllipseTemplate"] as DataTemplate;
            return modelTemplateSelector.SelectTemplate(item, container);
        }
    }
}

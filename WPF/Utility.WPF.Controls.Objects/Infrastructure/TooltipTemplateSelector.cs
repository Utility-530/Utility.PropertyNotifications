using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Newtonsoft.Json.Linq;

namespace Utility.WPF.Controls.Objects
{
    public class ToolTipTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is JProperty jProperty && jProperty.Parent.First.Values<string>().First() is string str && Type.GetType(str) is Type _type)
            {
                return Utility.WPF.Factorys.TemplateGenerator.CreateDataTemplate(() =>
                {
                    return new BreadCrumbs { ItemsSource = new string[] { _type.Assembly.GetName().Name, _type.Namespace, _type.Name, jProperty.Name } };
                });
            }
            return Utility.WPF.Factorys.TemplateGenerator.CreateDataTemplate(() =>
            {
                var x = new TextBlock { };
                x.Text = "No tooltip";
                return x;
            });
        }
    }
}
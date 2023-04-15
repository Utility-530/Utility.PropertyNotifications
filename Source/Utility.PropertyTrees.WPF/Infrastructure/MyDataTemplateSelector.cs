using System;
using System.Windows;
using System.Windows.Controls;
using Utility.PropertyTrees.Abstractions;

namespace Utility.PropertyTrees.WPF.Infrastructure
{
    internal class TemplateConstants
    {
        public const string FileName = @"C:\Users\rytal\source\repos\UtilityStandard\Utility.PropertyTrees\SoftFluent.Windows\Utility.PropertyTrees.WPF\Infrastructure\Templates.xaml";
    }

    public class MyDataTemplateSelector : DataTemplateSelector
    {
        private ResourceDictionary? resourceDictionary;

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            //FileStream s = new FileStream(TemplateConstants.FileName, FileMode.Open);
            //var resourceDictionary = (ResourceDictionary)XamlReader.Load(s);

            if (item is IProperty { ViewModel: ViewModel { Template: { DataTemplateKey: var key } viewModel } property })
            {
            }
            else
            {
                throw new Exception(" 54534 44");
            }

            resourceDictionary ??= new ResourceDictionary
            {
                Source = new Uri(TemplateConstants.FileName, UriKind.Absolute)
            };

            if (key == null)
                return base.SelectTemplate(item, container);

            var template = resourceDictionary[key] as DataTemplate;

            return template ?? base.SelectTemplate(item, container);
        }

        public static MyDataTemplateSelector Instance { get; } = new MyDataTemplateSelector();
    }
}
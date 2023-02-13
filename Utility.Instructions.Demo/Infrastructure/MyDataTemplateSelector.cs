using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using Utility.Trees;

namespace Utility.Instructions.Demo.Infrastructure
{

    class TemplateConstants
    {
        public const string FileName = @"C:\Users\rytal\source\repos\UtilityStandard\Utility.Instructions\Utility.Instructions.Resources\Templates.xaml";
    }

    public class MyDataTemplateSelector : DataTemplateSelector
    {

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            //FileStream s = new FileStream(TemplateConstants.FileName, FileMode.Open);
            //var resourceDictionary = (ResourceDictionary)XamlReader.Load(s);

            var resourceDictionary = new ResourceDictionary
            {
                Source = new Uri(TemplateConstants.FileName, UriKind.Absolute)
            };

            //var key = "TreeItemTemplate";
            var key = "TreeItemTemplate";
            //var key = new DataTemplateKey(typeof(Tree));
            var template = resourceDictionary[key] as DataTemplate;

            return template;
        }

        public static MyDataTemplateSelector Instance { get; } = new MyDataTemplateSelector();
    }


  

}

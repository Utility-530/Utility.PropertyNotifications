using System;
using System.Windows;
using Utility.Interfaces.NonGeneric;

namespace Utility.WPF.Templates
{
    public partial class Templates : ResourceDictionary
    {
        public Templates()
        {
        }
    }


    public class CustomDataTemplateSelector : GenericDataTemplateSelector
    {
        private ValueDataTemplateSelector? valueDataTemplateSelector;

        public override ResourceDictionary Templates
        {
            get
            {
                var uri = $"/{typeof(Templates).Namespace};component/{nameof(Templates)}.xaml";
                var resourceDictionary = new ResourceDictionary
                {
                    Source = new Uri(uri, UriKind.RelativeOrAbsolute)
                };
                return resourceDictionary;
            }
        }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var template = Select_Template(item, container);
            return template;
        }


        public DataTemplate Select_Template(object item, DependencyObject container)
        {

            if (item == null)
                return NullDataTemplate ??= NullTemplate();

      
            if (item is IValue {  } ivalue)
            {
                return (valueDataTemplateSelector ??= new ValueDataTemplateSelector()).SelectTemplate(item, container);
            }

            var type = item.GetType();

            if (new DataTemplateKey(type) is var key &&
                (container as FrameworkElement)?.TryFindResource(key) is DataTemplate dataTemplate)
                return dataTemplate;

            var interfaces = type.GetInterfaces();

            return base.SelectTemplate(item, container);
        }



        public static CustomDataTemplateSelector Instance => new();
    }
}
using System;
using System.Windows;
using System.Windows.Controls;
using Utility.Interfaces.NonGeneric;

namespace Utility.WPF.Templates
{
    public partial class Templates : ResourceDictionary
    {
        public Templates()
        {
        }
    }


    public class CustomDataTemplateSelector : DataTemplateSelector
    {
        private DataTemplateSelector? valueDataTemplateSelector, readOnlyValueDataTemplateSelector;

        public ResourceDictionary Templates
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


            //if (item == null)
            //    return NullDataTemplate ??= NullTemplate();


            if (item is IValue { } ivalue)
            {

                if (item is IIsReadOnly { IsReadOnly: false })
                {
                    return (valueDataTemplateSelector ??= new ValueDataTemplateSelector()).SelectTemplate(item, container);
                }

                return (readOnlyValueDataTemplateSelector ??= new ReadOnlyValueDataTemplateSelector()).SelectTemplate(item, container);
            }



            var type = item?.GetType();

            //if (type is Type && new DataTemplateKey(type) is var key &&
            //    (container as FrameworkElement)?.TryFindResource(key) is DataTemplate dataTemplate)
            //    return dataTemplate;

            //var interfaces = type.GetInterfaces();

            return( Templates["Missing"] as DataTemplate) ?? throw new Exception("dfs 33091111111");
        }



        public static CustomDataTemplateSelector Instance => new();
    }
}
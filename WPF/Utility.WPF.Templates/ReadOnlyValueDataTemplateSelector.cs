using System;
using System.Windows;
using Utility.Interfaces.NonGeneric;

namespace Utility.WPF.Templates
{
    public partial class ReadOnlyValueTemplates : ResourceDictionary
    {
        public ReadOnlyValueTemplates()
        {
            //InitializeComponent();
        }
    }


    public class ReadOnlyValueDataTemplateSelector : GenericDataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is IType { Type: { } type })
            {
                return Templates[new DataTemplateKey(type)] as DataTemplate;
            }

            if (item is not IValue { Value: var value })
            {
                throw new Exception($"Unexpected type for item {item.GetType().Name}");
            }

            if (value == null)
                return TemplateSelector.CreateNullTemplate();

            //var type = value.GetType();
            //var _descriptor = item is IPropertyDescriptor descriptor ? descriptor : null;
            //var _info = item is IPropertyInfo propertyInfo ? propertyInfo : null;


            return (Templates["Missing"] as DataTemplate) ?? throw new Exception("dfs 33091111111");
        }


        public override ResourceDictionary Templates
        {
            get
            {
                var resourceDictionary = new ResourceDictionary
                {
                    Source = new Uri($"/{typeof(ValueTemplates).Namespace};component/{nameof(ValueTemplates)}.xaml", UriKind.RelativeOrAbsolute)
                };
                return resourceDictionary;
            }
        }

        public static ValueDataTemplateSelector Instance => new();
    }
}
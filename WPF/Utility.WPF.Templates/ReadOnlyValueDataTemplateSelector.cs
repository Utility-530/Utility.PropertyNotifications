using System;
using System.Collections;
using System.Drawing;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Utility.Interfaces.NonGeneric;
using Utility.WPF.Helpers;

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
            var template = Select_Template(item, container);
            return template;
        }

        public DataTemplate Select_Template(object item, DependencyObject container)
        {

            if (item is not IValue { Value: var value })
            {
                throw new Exception($"Unexpected type for item {item.GetType().Name}");
            }

            if (value == null)
                return NullDataTemplate ??= NullTemplate();

            var type = value.GetType();
            var _descriptor = item is IPropertyDescriptor descriptor ? descriptor : null;
            var _info = item is IPropertyInfo propertyInfo ? propertyInfo : null;


            return base.SelectTemplate(value, container);
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
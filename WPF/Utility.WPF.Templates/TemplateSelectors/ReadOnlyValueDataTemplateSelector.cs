using System;
using System.Windows;
using Utility.Interfaces.NonGeneric;

namespace Utility.WPF.Templates
{


    public class ReadOnlyValueDataTemplateSelector : GenericDataTemplateSelector
    {
        private ReadOnlyValueTemplates valueTemplates;

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is not IValue { Value: var value })
            {
                throw new Exception($"Unexpected type for item {item.GetType().Name}");
            }
            if (item is IType { Type: { } type })
            {
                return Templates[new DataTemplateKey(type)] as DataTemplate;
            }
            if (value == null)
                return TemplateFactory.CreateNullTemplate();

            return (Templates["Missing"] as DataTemplate) ?? throw new Exception("dfs 33091111111");
        }


        public override ResourceDictionary Templates
        {
            get
            {
                //var resourceDictionary = new ResourceDictionary
                //{
                //    Source = new Uri($"/{typeof(ReadOnlyValueTemplates).Namespace};component/{nameof(ReadOnlyValueTemplates)}.xaml", UriKind.RelativeOrAbsolute)
                //};
                //return resourceDictionary;
                if (valueTemplates == null)
                {
                    valueTemplates = new ReadOnlyValueTemplates();
                    valueTemplates.InitializeComponent();
                }
                return valueTemplates;

            }
        }

        public static ValueDataTemplateSelector Instance => new();
    }
}
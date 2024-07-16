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
                if (FromType(type) is DataTemplate template)
                    return template;
            }
            if (value == null)
                return TemplateFactory.CreateNullTemplate();

            return (Templates["Missing"] as DataTemplate) ?? throw new Exception("dfs 33091111111");
        }


        public DataTemplate FromType(Type type)
        {
            if (Nullable.GetUnderlyingType(type) != null)
            {
                var underlyingType = Nullable.GetUnderlyingType(type);
                var underlyingTypeName = underlyingType.BaseType == typeof(Enum) ? underlyingType.BaseType.Name : underlyingType.Name;
                if (Templates[$"{type.Name}[{underlyingTypeName}]"] is DataTemplate dt)
                    return dt;
            }
            if (type.BaseType == typeof(Enum))
            {
                if (Templates[new DataTemplateKey(type.BaseType)] is DataTemplate __dt)
                    return __dt;
            }

            if (type == typeof(object))
            {
                return Templates["Object"] as DataTemplate;
            }

            if (Templates.Contains(new DataTemplateKey(type)))
                return Templates[new DataTemplateKey(type)] as DataTemplate;

            return null;
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
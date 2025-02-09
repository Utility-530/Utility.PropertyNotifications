using System;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using Utility.Interfaces.NonGeneric;

namespace Utility.WPF.Templates
{
    public class CustomDataTemplateSelector : DataTemplateSelector
    {
        private DataTemplateSelector? valueDataTemplateSelector, readOnlyValueDataTemplateSelector;
        private Templates templates;

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is IValue { } ivalue)
            {
                if (item is IIsReadOnly { IsReadOnly: false })
                {
                    return (valueDataTemplateSelector ??= new ValueDataTemplateSelector()).SelectTemplate(item, container);
                }
                else
                {
                    MethodInfo setMethod = item.GetType().GetProperty(nameof(IValue.Value)).GetSetMethod();

                    if (setMethod == null)
                    {
                        // The setter doesn't exist or isn't public.
                        return (readOnlyValueDataTemplateSelector ??= new ReadOnlyValueDataTemplateSelector()).SelectTemplate(item, container);
                    }
                    else
                    {
                        return (valueDataTemplateSelector ??= new ValueDataTemplateSelector()).SelectTemplate(item, container);
                    }
                }    
            }

            if (item is DataTemplate dataTemplate)
                return dataTemplate;

            if (item is null)
                return TemplateFactory.CreateNullTemplate();

            var type = item.GetType();

            if (FromType(type) is DataTemplate template)
                return template;

            return (Templates["Missing"] as DataTemplate) ?? throw new Exception("dfs 33091111111");

        }

        public ResourceDictionary Templates
        {
            get
            {
                if (templates == null)
                {
                    templates = Utility.WPF.Templates.Templates.Instance;
                    templates.InitializeComponent();
                }
                return templates;
            }
        }

        //public ResourceDictionary Templates
        //{
        //    get
        //    {
        //        var uri = $"/{typeof(Templates).Namespace};component/{nameof(Templates)}.xaml";
        //        var resourceDictionary = new ResourceDictionary
        //        {
        //            Source = new Uri(uri, UriKind.RelativeOrAbsolute)
        //        };
        //        return resourceDictionary;
        //    }
        //}

        public static CustomDataTemplateSelector Instance => new();


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
    }
}
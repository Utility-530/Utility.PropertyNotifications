using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Utility.WPF.Helpers
{
    public static class DataTemplateHelper
    {
        public static IEnumerable<DictionaryEntry> DefaultDataTemplates(this Type type)
        {
            var dataTemplateKey = new DataTemplateKey(type);
            var dt = (DataTemplate)Application.Current.Resources[dataTemplateKey];
            yield return new DictionaryEntry("Default", dt);
        }

        public static IEnumerable<DictionaryEntry> CustomDataTemplates(this Type type, ResourceDictionary res)
        {
            foreach (var entry in res.Cast<DictionaryEntry>())
            {
                var (key, value) = entry;
                if (value is DataTemplate { DataType: Type datatype })
                {
                    if (datatype.IsAssignableFrom(type))
                    {
                        yield return entry;
                    }
                }
            }
        }
        public static DataTemplateSelector CreateSelector(Func<object, DependencyObject, DataTemplate> factory)
        {
            return new CustomDataTemplateSelector(factory);
        }

        public static DataTemplate FindTemplate(this Type currentType, ResourceDictionary resourceDictionary)
        {
            DataTemplate baseTemplate = null;
            while (currentType != typeof(object))
            {
                baseTemplate = (DataTemplate)resourceDictionary[new DataTemplateKey(currentType)];
                if (baseTemplate != null)
                    break;

                currentType = currentType.BaseType;
            }

            return baseTemplate;
        }

        private class CustomDataTemplateSelector: DataTemplateSelector
        {
            private readonly Func<object, DependencyObject, DataTemplate> create;

            public CustomDataTemplateSelector(Func<object, DependencyObject, DataTemplate> create)
            {
                this.create = create;
            }
            public override DataTemplate SelectTemplate(object item, DependencyObject container)
            {
                return create(item, container);
            }
        }
    }
}
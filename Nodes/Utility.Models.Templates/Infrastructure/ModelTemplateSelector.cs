using Utility.WPF.ResourceDictionarys;
using System.Windows;
using System.Windows.Controls;
using Utility.Interfaces.Exs;
using Utility.Models.Trees;
using Utility.WPF.Helpers;
using Utility.WPF.Templates;
using Utility.Interfaces.NonGeneric;

namespace Utility.Models.Templates
{
    public class ModelTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is null)
            {
                return Templates.Instance["NullTemplate"] as DataTemplate;
            }
            if (item is IDataTemplate { DataTemplate: { } _template })
            {
                if (FindTemplate(_template) is { } value)
                {
                    return value;
                }
            }
            if (item is IType { Type: { } type })
            {
                return ValueDataTemplateSelector.Instance.FromType(type);
            }

            DataTemplate baseTemplate = item.GetType().FindTemplate(Templates.Instance);
            return baseTemplate;
      
            static DataTemplate? FindTemplate(string template)
            {
                if (Templates.Instance[template] is DataTemplate _dataTemplate)
                    return _dataTemplate;
                if (Application.Current.Resources.FindResource(template) is DataTemplate dataTemplate)
                    return dataTemplate;
                return null;
            }
        }

        public static ModelTemplateSelector Instance { get; } = new();
    }
}

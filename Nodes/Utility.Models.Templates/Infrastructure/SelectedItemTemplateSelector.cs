using Utility.WPF.ResourceDictionarys;

namespace Utility.Models.Templates
{
    using System.Windows;
    using System.Windows.Controls;
    using Utility.Interfaces.Exs;
    using Utility.WPF.Helpers;

    public class SelectedItemTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if(item is null)
                return Templates.Instance["NullTemplate"] as DataTemplate ?? throw new Exception("ff3vcfdf3");

            if (item is ISelectedItemTemplate { SelectedItemTemplate: { } _template })
            {
                if (FindTemplate(_template) is { } value)
                {
                    return value;
                }
            }

            DataTemplate baseTemplate = item.GetType().FindTemplate(Templates.Instance);

            if (baseTemplate is HierarchicalDataTemplate)
            {
                return Templates.Instance["DefaultTemplate"] as DataTemplate ?? throw new Exception("d3091111111");
            }
            else
            {
                //if (baseTemplate.DataType is Type type && type == typeof(ListModel))
                //{
                return baseTemplate;
                //}
            }
            //throw new Exception("VSD 333");
            //return baseTemplate ?? Templates.Instance["Missing"] as DataTemplate ?? throw new Exception("d3091111111");

            static DataTemplate? FindTemplate(string template)
            {
                if (Templates.Instance[template] is DataTemplate _dataTemplate)
                    return _dataTemplate;
                if (Application.Current.Resources.FindResource(template) is DataTemplate dataTemplate)
                    return dataTemplate;
                return null;
            }
        }
    }
}

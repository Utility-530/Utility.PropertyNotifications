using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Tiny.Toolkits;
using Utility.Interfaces.NonGeneric;
using Utility.WPF.Controls.DataGrids;
using Utility.WPF.Demo.Common.ViewModels;
using Utility.WPF.Templates;

namespace Utility.WPF.Demo.DataGrids
{
    public class MyDataTemplateSelector : CustomDataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var type = item?.GetType();
            if (type?.FullName?.Equals("System.RuntimeType") == true)
            {
                return TypeTemplate;
            }
            if (item?.GetType().IsValueType == false && item is not string s)
            {
                var resource = container.GetResource<DataTemplate>("ObjectComboBoxTemplate");
                return resource;
            }

            return base.SelectTemplate(item, container);
        }

        public new static MyDataTemplateSelector Instance { get; } = new();

        public DataTemplate TypeTemplate { get; set; }
    }

    internal class TypeFilter : IPredicate
    {
        private readonly string[] names = new[] { "Name", "FullName" };

        public bool Evaluate(object value)
        {
            if (value is IPropertyInfo propertyInfo)
            {
                var contains = names.Contains(propertyInfo.PropertyInfo.Name);
                return contains;
            }
            return false;
        }
    }

    public class CustomDataGridColumnSelectorBehavior : DataGridColumnSelectorBehavior
    {
        public CustomDataGridColumnSelectorBehavior()
        {
            DataTemplateSelector = new MyDataTemplateSelector();
        }

        protected override DataGridColumn SelectColumn(DataGridAutoGeneratingColumnEventArgs e)
        {
            if (e.PropertyName.Equals(nameof(Region.Left)))
            {
                return new DataGridComboBoxColumn() { ItemsSource = Array.Empty<string>() };
            }

            return base.SelectColumn(e);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Tiny.Toolkits;
using Utility.Interfaces.NonGeneric;
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

        public static new MyDataTemplateSelector Instance { get; } = new();


        public DataTemplate TypeTemplate { get; set; }
    }

    class TypeFilter : IPredicate
    {
        readonly string[] names = new[] { "Name", "FullName" };
        public bool Invoke(object value)
        {
            if (value is IPropertyInfo propertyInfo)
            {
                var contains = names.Contains(propertyInfo.Property.Name);
                return contains;
            }
            return false;
        }
    }
}

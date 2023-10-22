using System;
using System.Windows;
using Tiny.Toolkits;
using Utility.Interfaces.NonGeneric;
using Utility.Models;

namespace Utility.WPF.Controls.Objects
{
    public class ValueObjectsDataTemplateSelector : Templates.CustomDataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            object value = item;
            if (item is IValue { Value: var _value })
            {
                value = _value;
            }

            var type = value?.GetType();
            if (type?.FullName?.Equals("System.RuntimeType") == true)
            {
                return TypeTemplate;
            }
            if (type?.IsValueType == false && value is not string s)
            {
                return ObjectComboBoxTemplate;
            }

            return base.SelectTemplate(item, container);
        }

        public DataTemplate TypeTemplate { get; set; }
        public DataTemplate ObjectComboBoxTemplate { get; set; }
    }

    //public class ValueObjectsDataTemplateSelector : Templates.CustomDataTemplateSelector
    //{
    //    public override DataTemplate SelectTemplate(object item, DependencyObject container)
    //    {
    //        if (item is ValueViewModel { Value: var _value } valueViewModel)
    //        {
    //            var type = _value?.GetType();             
    //            if (type?.FullName?.Equals("System.RuntimeType") == true)
    //            {
    //                return TypeTemplate;
    //            }
    //            if (type?.IsValueType == false && _value is not string s)
    //            {
    //                var resource = container.GetResource<DataTemplate>("ObjectComboBoxTemplate");
    //                return resource;
    //            }
    //            return base.SelectTemplate(_value, container);

    //        }

    //        return ErrorTemplate;
    //    }

    //    public DataTemplate TypeTemplate { get; set; }
    //    public DataTemplate ErrorTemplate { get; set; }
    //}
}
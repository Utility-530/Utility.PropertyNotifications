using System.Reflection;
using System.Windows;
using System.Windows.Data;

namespace Utility.WPF.Behaviors
{
    public class SetSelectedItemAction : SetterAction
    {

        protected override void Invoke(object parameter)
        {
            object value = null;

            if (Converter is IValueConverter converter)
            {
                value = converter.Convert(parameter, null, null, null);
            }
            else if (parameter is not RoutedPropertyChangedEventArgs<object> { NewValue: { } _value })
            {
                return;
            }
            else
            {
                value = _value;
            }
            object target = TargetObject ?? AssociatedObject;
            PropertyInfo propertyInfo = target.GetType().GetProperty(PropertyName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.InvokeMethod);
            propertyInfo.SetValue(target, value);
        }  
    }
}
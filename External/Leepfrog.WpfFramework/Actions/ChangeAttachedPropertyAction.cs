using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.ComponentModel;
using Microsoft.Xaml.Behaviors.Core;

namespace Leepfrog.WpfFramework.Actions
{
    public class ChangeAttachedPropertyAction : ChangePropertyAction
    {
        private class TemporaryBinding : DependencyObject
        {

            public object Value
            {
                get { return (object)GetValue(ValueProperty); }
                set { SetValue(ValueProperty, value); }
            }

            // Using a DependencyProperty as the backing store for Property.  This enables animation, styling, binding, etc...
            public static readonly DependencyProperty ValueProperty =
                DependencyProperty.Register("Value", typeof(object), typeof(ChangeAttachedPropertyAction), new PropertyMetadata(null));
        }

        public Type AttachedPropertyType
        {
            get { return (Type)GetValue(AttachedPropertyTypeProperty); }
            set { SetValue(AttachedPropertyTypeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AttachedPropertyType.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AttachedPropertyTypeProperty =
            DependencyProperty.Register("AttachedPropertyType", typeof(Type), typeof(ChangeAttachedPropertyAction), new PropertyMetadata(null));

        protected override void Invoke(object parameter)
        {
            var target = (DependencyObject)Target;
            var ownerType = AttachedPropertyType ?? target.GetType();
            var prop = DependencyPropertyDescriptor.FromName(PropertyName, ownerType, target.GetType()).DependencyProperty;
            var boundValue = BindingOperations.GetBinding(this, ChangeAttachedPropertyAction.ValueProperty);
            if (boundValue != null)
            {
                BindingOperations.SetBinding(
                    target,
                    prop,
                    boundValue
                    );
            }
            else
            {
                var value = Value;
                target.SetValue(prop, convert(value, prop.PropertyType));
            }
        }

        private object convert(object value, Type targetType)
        {
            if (targetType.IsInstanceOfType(value))
            {
                return value;
            }

            TypeConverter converter = TypeDescriptor.GetConverter(targetType);

            try
            {
                // determine if the supplied value is of a suitable type
                if (converter.CanConvertFrom(value.GetType()))
                {
                    // return the converted value
                    return converter.ConvertFrom(value);
                }
                else
                {
                    // try to convert from the string representation
                    return converter.ConvertFrom(value.ToString());
                }
            }
            catch (Exception)
            {
                return value;
            }
        }

    }
}

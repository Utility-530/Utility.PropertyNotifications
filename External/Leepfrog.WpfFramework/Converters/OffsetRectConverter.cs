using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
using System.ComponentModel;
using System.Windows.Controls;
using System.Collections;

namespace Leepfrog.WpfFramework.Converters
{
    public class OffsetRectConverter : IMultiValueConverter
    {

        class DummyDO : DependencyObject
        {
            public object Value
            {
                get { return (object)GetValue(ValueProperty); }
                set { SetValue(ValueProperty, value); }
            }

            public static readonly DependencyProperty ValueProperty =
                DependencyProperty.Register("Value", typeof(object), typeof(DummyDO), new UIPropertyMetadata(null));

        }

        public object EvalBinding(BindingBase b)
        {
            // TODO: Put DummyDO into a global class
            // TODO: Make this a one-time only binding?
            DummyDO d = new DummyDO();
            BindingOperations.SetBinding(d, DummyDO.ValueProperty, b);
            var value = d.Value;
            BindingOperations.ClearBinding(d, DummyDO.ValueProperty);
            return value;
        }


        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            //=================================================================
            try
            {
                //-----------------------------------------------------------------
                // WORKAROUNDS FOR BINDTO EXTENSION AT DESIGN TIME!
                if (DesignerProperties.GetIsInDesignMode(new DependencyObject()))
                {
                    //-----------------------------------------------------------------
                    var target = new Target();
                    //-----------------------------------------------------------------
                    // loop through all in values
                    for (var i = values.Length - 1; i >= 2; i--)
                    {
                        var inx = values[i];
                        if (inx is MarkupExtension)
                        {
                            inx = (inx as MarkupExtension).ProvideValue(target);
                        }
                        if (inx is BindingBase)
                        {
                            inx = EvalBinding(inx as BindingBase);
                        }
                        values[i] = inx;
                    }
                    //-----------------------------------------------------------------
                }
                //-----------------------------------------------------------------
                var rect = new Rect(toDouble(values[0]), toDouble(values[1]), toDouble(values[2]), toDouble(values[3]));
                var vector = new Vector(-toDouble(values[4]), -toDouble(values[5]));
                rect.Offset(vector);
                return rect;
                //-----------------------------------------------------------------
                //-----------------------------------------------------------------
            }
            catch (Exception ex)
            {
                return ex.ToString();
                throw;
            }
            //-----------------------------------------------------------------
        }

        private double toDouble(object from)
        {
            if (from == null)
            {
                return 0;
            }
            return ((from as IConvertible).ToDouble(null));
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
                else if (value == DependencyProperty.UnsetValue)
                {
                    return value;
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

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public struct Target : IServiceProvider, IProvideValueTarget
        {
            private readonly DependencyObject _targetObject;
            private readonly DependencyProperty _targetProperty;

            public Target(DependencyObject targetObject, DependencyProperty targetProperty)
            {
                _targetObject = targetObject;
                _targetProperty = targetProperty;
            }

            public object GetService(Type serviceType)
            {
                if (serviceType == typeof(IProvideValueTarget))
                    return this;
                return null;
            }

            object IProvideValueTarget.TargetObject { get { return _targetObject; } }
            object IProvideValueTarget.TargetProperty { get { return _targetProperty; } }
        }
    }
}

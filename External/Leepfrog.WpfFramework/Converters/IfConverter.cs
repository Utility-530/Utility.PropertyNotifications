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
using Microsoft.Xaml.Behaviors.Core;
using Microsoft.Xaml.Behaviors;


namespace Leepfrog.WpfFramework.Converters
{
    public class IfConverter : IMultiValueConverter
    {
        internal ComparisonConditionType _comparison = ComparisonConditionType.Equal;


        /// <summary>
        /// Value converter that performs a simple if in1 = in2 then ... else ...
        /// param1 = in1
        /// param2 = in2
        /// param3 = return if true
        /// param4 = return if false
        /// </summary>
        /// <remarks>
        /// </remarks>


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
        public string Debug { get; internal set; }

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
            if (!string.IsNullOrEmpty(Debug))
            {
                //just for breakpoints!
            }
            string debug = "";
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
                /*
                // EXPAND AN ENUMERABLE - ie model repository
                // removed in case it caused side effects
                if (
                    (!(values[3] is string))
                 && (values[3] is IEnumerable enumerable)
                   )
                {
                    var newValues = values.Take(3).ToList();
                    foreach (var item in enumerable)
                    {
                        newValues.Add(item);
                    }
                    values = newValues.ToArray();
                }
                */
                //-----------------------------------------------------------------
                var returnIfTrue = values[0];
                var returnIfFalse = values[1];
                var in1 = values[2];
                var toType = in1?.GetType() ?? typeof(object);
                //-----------------------------------------------------------------
                foreach (var in2 in values.Skip(3))
                {
                    //-----------------------------------------------------------------
                    debug = "1";
                    if ((in1 == null) || (in1 == DependencyProperty.UnsetValue))
                    {
                        debug = "9";
                        return (in2 == null) ? convert(returnIfTrue, targetType) : convert(returnIfFalse, targetType);
                    }
                    debug = "2";
                    if (in2 == null) { }
                    //-----------------------------------------------------------------
                    //-----------------------------------------------------------------
                    debug = "3";
                    bool isMatch = false;
                    //-----------------------------------------------------------------
                    if (_comparison != ComparisonConditionType.Equal)
                    {
                        try
                        {
                            isMatch = ComparisonLogic.EvaluateImpl(in1, _comparison, in2);
                        }
                        catch (Exception)
                        {
                            isMatch = false;
                        }
                    }
                    else if (toType.IsEnum)
                    {
                        debug = "7";
                        isMatch = ((in2 is string) && (in1.Equals(Enum.Parse(toType, (string)in2))));
                    }
                    else if (in2 is IConvertible in2Convertible)
                    {
                        debug = "8";
                        try
                        {
                            isMatch = (in1.Equals(in2Convertible.ToType(toType, culture)));
                        }
                        catch { }
                    }
                    else if ((in1 != null) && (in2 != null) && (in1.GetType().IsValueType) && (in2.GetType().IsValueType))
                    {
                        debug = "8a";
                        isMatch = in1.Equals(in2);
                    }
                    else
                    {
                        debug = "9";
                        isMatch = (in1 == in2);
                    }
                    //-----------------------------------------------------------------
                    if (isMatch)
                    {
                        debug = "5";
                        return convert(returnIfTrue, targetType);
                    }
                    debug = "6";
                    //-----------------------------------------------------------------
                }
                //-----------------------------------------------------------------
                // none of the possible in2s match, return false
                return convert(returnIfFalse, targetType);
                //-----------------------------------------------------------------
            }
            catch (Exception ex)
            {
                this.AddLog("IfConverter", $"debug {Debug}.{debug}, exception {ex.ToString()}");
                this.AddLog(ex);
                throw;
            }
            //-----------------------------------------------------------------
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

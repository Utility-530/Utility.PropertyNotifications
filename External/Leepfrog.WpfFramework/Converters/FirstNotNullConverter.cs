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
using System.Diagnostics;

namespace Leepfrog.WpfFramework.Converters
{
    public class FirstNotNullConverter : IMultiValueConverter
    {
        internal string Debug;

        /// <summary>
        /// Value converter that performs a simple if in1 !=  null then in1 else if in2 != null then in 2 ...
        /// param1 = in1
        /// param2 = in2
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

        private void AddLog(string header, string data)
        {
            if (Debug == "TEST")
            {
                Logger.AddLog(this,header,data);
            }
        }
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            //=================================================================
            try
            {
                //-----------------------------------------------------------------
                this.AddLog("FIRSTNOTNULL", "STARTING");
                foreach ( var value in values )
                {
                    //-----------------------------------------------------------------
                    this.AddLog("FIRSTNOTNULL", $"- CHECKING {value}");
                    var check = value;
                    //-----------------------------------------------------------------
                    // WORKAROUNDS FOR BINDTO EXTENSION AT DESIGN TIME!
                    if ( DesignerProperties.GetIsInDesignMode(new DependencyObject()) )
                    {
                        //-----------------------------------------------------------------
                        if ( check is MarkupExtension )
                        {
                            var target = new Target();
                            check = ( value as MarkupExtension ).ProvideValue(target);
                        }
                        //-----------------------------------------------------------------
                        if ( check is BindingBase )
                        {
                            check = EvalBinding(check as BindingBase);
                        }
                        //-----------------------------------------------------------------
                    }
                    //-----------------------------------------------------------------
                    if (( check == null ) || (check == DependencyProperty.UnsetValue) ){ }
                    else if ( ( check is string checkString ) && ( string.IsNullOrWhiteSpace(checkString) ) ) { }
                    else
                    {
                        //-----------------------------------------------------------------
                        this.AddLog("FIRSTNOTNULL", $"- returning {check}");
                        return convert(check,targetType);
                        //-----------------------------------------------------------------
                    }
                    //-----------------------------------------------------------------
                }
                //-----------------------------------------------------------------
                this.AddLog("FIRSTNOTNULL", $"- failed");
                return "no matches ";
                //-----------------------------------------------------------------
            }
            catch ( Exception ex )
            {
                this.AddLog("FIRSTNOTNULL", $"exception {ex.Message}");
                throw;
            }
            //-----------------------------------------------------------------
        }
        private object convert(object value, Type targetType)
        {
            if ( targetType.IsInstanceOfType(value) )
            {
                return value;
            }

            TypeConverter converter = TypeDescriptor.GetConverter(targetType);

            try
            {
                // determine if the supplied value is of a suitable type
                if ( converter.CanConvertFrom(value.GetType()) )
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
            catch ( Exception )
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
                if ( serviceType == typeof(IProvideValueTarget) )
                    return this;
                return null;
            }

            object IProvideValueTarget.TargetObject { get { return _targetObject; } }
            object IProvideValueTarget.TargetProperty { get { return _targetProperty; } }
        }
    }
}

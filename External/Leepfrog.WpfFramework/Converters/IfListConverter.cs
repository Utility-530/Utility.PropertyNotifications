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

using System.Collections.Specialized;

namespace Leepfrog.WpfFramework.Converters
{
    public class IfListConverter : IMultiValueConverter
    {
        /// <summary>
        /// Value converter that checks against a list of items if in1 = in2 then ... else ...
        /// param0 = in2
        /// param1 = return if true
        /// param2 = return if false
        /// param3 = markupextension(the markup extension currently controls adding/deleting of these items, hopefully this will change?!)
        /// param4 = list
        /// </summary>
        /// <remarks>
        /// </remarks>

        public enum AnyAll
        {
            Any,
            All,
            AllNotNone,
            AnyOrNone
        }

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


        public IfListConverter()
        {

        }

        private AnyAll _type;
        private PropertyPath _path;
        //private bool _listLoaded;
        
        public IfListConverter(AnyAll type, PropertyPath path)
        {
            _type = type;
            _path = path;
        }


        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            //this.AddLog("IFLISTCONV", "CONVERTING");
            string debug = "";
            //=================================================================
            try
            {
                //-----------------------------------------------------------------
                //if ( !_listLoaded )
                {
                    //_listLoaded = true;
                    var multiBinding = values[3] as IfListMultiBinding;
                    multiBinding.setCollection(values[4] as INotifyCollectionChanged);
                }
                //-----------------------------------------------------------------
                var in2 = values[0];
                //this.AddLog("IFLISTCONV", $"(in2 = {in2})");
                //-----------------------------------------------------------------
                // WORKAROUNDS FOR BINDTO EXTENSION AT DESIGN TIME!
                if ( DesignerProperties.GetIsInDesignMode(new DependencyObject()) )
                {
                    var target = new Target();
                    if ( in2 is MarkupExtension )
                    {
                        in2 = ( in2 as MarkupExtension ).ProvideValue(target);
                    }
                    if ( in2 is BindingBase )
                    {
                        in2 = EvalBinding(in2 as BindingBase);
                    }
                }
                //-----------------------------------------------------------------
                var returnIfTrue = values[1];
                var returnIfFalse = values[2];
                //-----------------------------------------------------------------
                Func<object, object, bool> func =
                    (check1, check2) =>
                    {
                        //-----------------------------------------------------------------
                        // IF THIS VALUE IS NULL...
                        if ( ( check1 == null ) || ( check1 == DependencyProperty.UnsetValue ) )
                        {
                            debug = "9";
                            return ( ( check2 == null ) || ( check2 == DependencyProperty.UnsetValue ) );
                        }
                        //-----------------------------------------------------------------
                        debug = "2";
                        var toType = check1.GetType();
                        //-----------------------------------------------------------------
                        if ( toType.IsEnum )
                        {
                            debug = "7";
                            return ( ( check2 is string ) && ( check1.Equals(Enum.Parse(toType, (string)check2)) ) );
                        }
                        else if (check2 is IConvertible)
                        {
                            debug = "8";
                            return ((check2 is IConvertible) && (check1.Equals(((IConvertible)check2).ToType(toType, culture))));
                        }
                        else if ((check1 != null) && (check2 != null) && (check1.GetType().IsValueType) && (check2.GetType().IsValueType))
                        {
                            debug = "8a";
                            return (check1.Equals(check2));
                        }
                        else
                        {
                            debug = "9";
                            return (check2.Equals(check1));
                        }
                    };
                //-----------------------------------------------------------------
                // IF WE'RE CHECKING AGAINST AN ARRAY, CHECK WHETHER ANY ITEM MATCHES...
                Func<object, bool> func2 =
                    (check1) =>
                    {
                        if ( in2 is object[] )
                        {
                            debug = "4";
                            return ( (object[])in2 ).Any(check2 => func(check1, check2));
                        }
                        else
                        {
                            return func(check1, in2);
                        }
                    };
                //-----------------------------------------------------------------
                debug = "1";
                bool atLeastOne = false;
                //-----------------------------------------------------------------
                // LOOP THROUGH THE 'ITEMS' 
                var list = values[4] as IEnumerable;
                if (list == null)
                {
                    // the list is not a list!
                    return null;
                }
                //-----------------------------------------------------------------
                // ADDED ML 2019-08-23
                // we might need to lock the list
                // so let's lock it, take a copy, then loop through the copy instead
                var copyOfList = new List<object>();
                BindingOperations.AccessCollection(
                    list,
                    ()=> { foreach (var item in list) { copyOfList.Add(item); } },
                    false
                    );
                //-----------------------------------------------------------------
                foreach ( var item in copyOfList )
                {
                    //-----------------------------------------------------------------
                    atLeastOne = true;
                    //-----------------------------------------------------------------
                    var binding = new Binding();
                    binding.Source = item;
                    binding.Path = _path;
                    var in1 = EvalBinding(binding);
                    //this.AddLog("IFLISTCONV", $"CHECKING {in1}");
                    //-----------------------------------------------------------------
                    if ( ( _type == AnyAll.All ) || ( _type == AnyAll.AllNotNone ) )
                    {
                        if ( !func2(in1) )
                        {
                            //this.AddLog("IFLISTCONV", $"LOOKING FOR {_type}, THIS ONE DOESN'T MATCH, RETURNING {returnIfFalse}");
                            return convert(returnIfFalse, targetType);
                        }
                    }
                    else
                    {
                        if ( func2(in1) )
                        {
                            //this.AddLog("IFLISTCONV", $"LOOKING FOR {_type}, THIS ONE MATCHES, RETURNING {returnIfTrue}");
                            return convert(returnIfTrue, targetType);
                        }
                    }
                    //-----------------------------------------------------------------
                }
                //-----------------------------------------------------------------
                debug = "6";
                if (
                    ( _type == AnyAll.All )
                 || (
                     ( _type == AnyAll.AllNotNone )
                  && ( atLeastOne )
                    )
                 || (
                     ( _type == AnyAll.AnyOrNone )
                  && ( !atLeastOne )
                    )
                   )
                {
                    //this.AddLog("IFLISTCONV", $"LOOKING FOR {_type}, OK, RETURNING {returnIfTrue}");
                    return convert(returnIfTrue, targetType);
                }
                else
                {
                    //this.AddLog("IFLISTCONV", $"LOOKING FOR {_type}, NO MATCHES, RETURNING {returnIfFalse}");
                    return convert(returnIfFalse, targetType);
                }
                //-----------------------------------------------------------------
            }
            catch ( Exception ex )
            {
                return $"ERROR AT {debug} {ex.ToString()}";// throw new Exception($"ERROR AT {debug}",ex);
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

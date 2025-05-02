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

namespace Leepfrog.WpfFramework.Converters
{
    /*
    public class IfConverter :
            MarkupExtension,
          IValueConverter
    {
        /// <summary>
        /// Value converter that performs a simple if ... then ... else
        /// </summary>
        /// <remarks>
        /// </remarks>

        private object _check;
        private object _returnIfTrue;
        private object _returnIfFalse;

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
            DummyDO d = new DummyDO();
            BindingOperations.SetBinding(d, DummyDO.ValueProperty, b);
            return d.Value;
        }

        public IfConverter()
        {
        }

        public IfConverter(object check, object returnIfTrue, object returnIfFalse)
        {
            _check = check;

            _returnIfTrue = returnIfTrue;
            _returnIfFalse = returnIfFalse;

        }
        
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //=================================================================
            if (value == null)
            {
                return (_check == null) ? unwrapBinding(_returnIfTrue) : unwrapBinding(_returnIfFalse);
            }
            //-----------------------------------------------------------------
            Func<object, bool> func =
                (check) =>
                {
                    if (value.GetType().IsEnum)
                    {
                        return ((check is string) && (value.Equals(Enum.Parse(value.GetType(), (string)check))));
                    }
                    else
                    {
                        return ((check is IConvertible) && (value.Equals(((IConvertible)check).ToType(value.GetType(), culture))));
                    }
                };
            //-----------------------------------------------------------------
            // IF WE'RE CHECKING AGAINST AN ARRAY, CHECK WHETHER ANY ITEM MATCHES...
            if (_check is object[])
            {
                if (((object[])_check).Any(func))
                {
                    return unwrapBinding(_returnIfTrue);
                }
            }
            else if (func(_check))
            {
                return unwrapBinding(_returnIfTrue);
            }
            return unwrapBinding(_returnIfFalse);
            //-----------------------------------------------------------------
        }

        private object unwrapBinding(object ret)
        {
            if (ret is BindingBase)
            {
                return EvalBinding((BindingBase)ret);
            }
            else
            {
                return ret;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }
*/
}

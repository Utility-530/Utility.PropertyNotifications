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
using Leepfrog.WpfFramework.Converters;
using System.Collections;
using Microsoft.Xaml.Behaviors.Core;

namespace Leepfrog.WpfFramework
{
    public class ComparisonBindingExtension :
            MarkupExtension
    {
        /// <summary>
        /// Markup extension that returns a multibinding to do an IF then else
        /// </summary>
        /// <remarks>
        /// </remarks>

        public ComparisonBindingExtension()
        {

        }

        private MultiBinding _multiBinding;

        public ComparisonBindingExtension(object in1, ComparisonConditionType comparison, object in2, object returnIfTrue, object returnIfFalse)
            : this(in1, comparison, in2, returnIfTrue, returnIfFalse, null)
        { }

        public ComparisonBindingExtension(object in1, ComparisonConditionType comparison, object in2, object returnIfTrue, object returnIfFalse, object converter)
            : this(in1, comparison, in2,returnIfTrue,returnIfFalse,converter,converter)
        {
        }
        public ComparisonBindingExtension(object in1, ComparisonConditionType comparison, object in2, object returnIfTrue, object returnIfFalse, object converterIfTrue, object converterIfFalse) 
        {
            _multiBinding = new MultiBinding();
            _multiBinding.Mode = BindingMode.OneWay;
            _multiBinding.Converter = new IfConverter();
            _multiBinding.Bindings.Add(convertBinding(returnIfTrue, (IValueConverter)converterIfTrue));
            _multiBinding.Bindings.Add(convertBinding(returnIfFalse, (IValueConverter)converterIfFalse));
            _multiBinding.Bindings.Add(convertBinding(in1));
            if (
                (!(in2 is string))
             && (in2 is IEnumerable items)
                )
            {
                foreach (var item in items)
                {
                    _multiBinding.Bindings.Add(convertBinding(item));
                }
            }
            else
            {
                _multiBinding.Bindings.Add(convertBinding(in2));
            }
        }

        private BindingBase convertBinding(object param, IValueConverter conv = null)
        {
            Binding binding;
            if (param is Binding)
            {
                binding = (param as Binding);
            }
            else
            {
                binding = new Binding();
                binding.Source = param;
            }
            if (binding.Converter == null)
            {
                binding.Converter = conv;
            }
            return binding;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return _multiBinding.ProvideValue(serviceProvider);
        }

     }
}

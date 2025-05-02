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
using System.ComponentModel;

namespace Leepfrog.WpfFramework
{
    public class IfBindingExtension :
            MarkupExtension
    {
        /// <summary>
        /// Markup extension that returns a multibinding to do an IF then else
        /// </summary>
        /// <remarks>
        /// </remarks>
        public string Debug { get; set; }

        public IfBindingExtension()
        {

        }

        private MultiBinding _multiBinding;

        public IfBindingExtension(object in1, object in2, object returnIfTrue, object returnIfFalse)
            : this(in1, in2, returnIfTrue, returnIfFalse, null, null)
        { }

        public IfBindingExtension(object in1, ComparisonConditionType comparison, object in2, object returnIfTrue, object returnIfFalse)
            : this(in1, in2, returnIfTrue, returnIfFalse)
        {
            (_multiBinding.Converter as IfConverter)._comparison = comparison;
        }
        public IfBindingExtension(object in1, object in2, object returnIfTrue, object returnIfFalse, object converterIfTrue, object converterIfFalse)
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
            if (binding.Mode == BindingMode.Default)
            {
                binding.Mode = BindingMode.OneWay;
            }
            return binding;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            (_multiBinding.Converter as IfConverter).Debug = Debug;

            //var provideValueTarget = serviceProvider.GetService(typeof(IProvideValueTarget)) as IProvideValueTarget;
            //var target = provideValueTarget?.TargetObject as FrameworkElement;

            //var value = DesignerProperties.GetIsInDesignMode(target);

            //if (value == DependencyProperty.UnsetValue || value == null)
            //    return value;

            for (var i = 0; i < _multiBinding.Bindings.Count; i++)
            {
                if (_multiBinding.Bindings[i] is Binding b)
                {
                    if (b.Source is MarkupExtension mx)
                    {
                        b.Source = mx.ProvideValue(serviceProvider);
                        if (b.Source is BindingExpression bexp)
                        {
                            _multiBinding.Bindings[i] = bexp.ParentBinding;
                        }
                    }
                }
            }

            return _multiBinding.ProvideValue(serviceProvider);
        }

    }
}

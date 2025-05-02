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

namespace Leepfrog.WpfFramework.Converters
{
    public class MathBindingExtension :
            MarkupExtension
    {
        /// <summary>
        /// Markup extension that returns a multibinding to do a math calculation
        /// </summary>
        /// <remarks>
        /// </remarks>

        private object _fallback;
        private object _null;
        private object _param;
        private string _format;
        private IValueConverter _converter;

        public object Fallback { get { return _fallback; } set { _fallback = value; } }
        public object Null { get { return _null; } set { _null = value; } }
        public object Nullback { get { return _fallback; } set { _null = value; _fallback = value; } }
        public object Param { get { return _param; } set { _param = value; } }
        public string Format { get { return _format; } set { _format = value; } }
        public IValueConverter Converter { get { return _converter; } set { _converter = value; } }


        public MathBindingExtension()
        {

        }

        private MultiBinding _multiBinding;

        public MathBindingExtension(string formula, object in1)
        {
            _multiBinding = new MultiBinding();
            _multiBinding.Mode = BindingMode.OneWay;
            _multiBinding.Converter = new MathConverter();
            _multiBinding.ConverterParameter = formula;
            _multiBinding.Bindings.Add(convertBinding(in1));
        }

        public MathBindingExtension(string formula, object in1, object in2) : this(formula, in1)
        {
            _multiBinding.Bindings.Add(convertBinding(in2));
        }
        public MathBindingExtension(string formula, object in1, object in2, object in3) : this(formula, in1, in2)
        {
            _multiBinding.Bindings.Add(convertBinding(in3));
        }

        public MathBindingExtension(string formula, object in1, object in2, object in3, object in4) : this(formula, in1, in2, in3)
        {
            _multiBinding.Bindings.Add(convertBinding(in4));
        }
        public MathBindingExtension(string formula, object in1, object in2, object in3, object in4, object in5) : this(formula, in1, in2, in3, in4)
        {
            _multiBinding.Bindings.Add(convertBinding(in5));
        }

        private BindingBase convertBinding(object param)
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
            return binding;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            var conv = (_multiBinding.Converter as MathConverter);
            if (_format != null)
            {
                _multiBinding.StringFormat = _format;
                conv.StringFormat = _format;
            }
            if (_fallback != null)
            {
                _multiBinding.FallbackValue = _fallback;
            }
            if (_null != null)
            {
                _multiBinding.TargetNullValue = _null;
            }
            if (_converter != null)
            {
                conv.Converter = _converter;
                conv.ConverterParameter = _param;
            }
            return _multiBinding.ProvideValue(serviceProvider);
        }

    }
}

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
    public class AnyChangeExtension :
            MarkupExtension
    {
        /// <summary>
        /// Markup extension that returns a multibinding to trigger a propertychangedtrigger whenever any of the children raise propertychanged
        /// </summary>
        /// <remarks>
        /// </remarks>

        public AnyChangeExtension()
        {

        }

        private MultiBinding _multiBinding;

        public AnyChangeExtension(object in1, object in2)
        {
            _multiBinding = new MultiBinding();
            _multiBinding.Mode = BindingMode.OneWay;
            _multiBinding.Converter = new AnyChangeConverter();
            _multiBinding.Bindings.Add(convertBinding(in1));
            _multiBinding.Bindings.Add(convertBinding(in2));
        }

        public AnyChangeExtension(object in1, object in2, object in3) : this(in1, in2)
        {
            _multiBinding.Bindings.Add(convertBinding(in3));
        }

        public AnyChangeExtension(object in1, object in2, object in3, object in4) : this(in1, in2, in3)
        {
            _multiBinding.Bindings.Add(convertBinding(in4));
        }

        public AnyChangeExtension(object in1, object in2, object in3, object in4, object in5) : this(in1, in2, in3, in4)
        {
            _multiBinding.Bindings.Add(convertBinding(in5));
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

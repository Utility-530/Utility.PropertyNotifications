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

namespace Leepfrog.WpfFramework
{
    public class FirstNotNullExtension :
            MarkupExtension
    {
        /// <summary>
        /// Markup extension that returns a multibinding to do an IF then else
        /// </summary>
        /// <remarks>
        /// </remarks>

        public FirstNotNullExtension()
        {

        }

        private MultiBinding _multiBinding;

       
        
        public FirstNotNullExtension(object in1, object in2)
        {
            _multiBinding = new MultiBinding();
            _multiBinding.Mode = BindingMode.OneWay;
            _multiBinding.Converter = new FirstNotNullConverter();
            _multiBinding.Bindings.Add(convertBinding(in1));
            _multiBinding.Bindings.Add(convertBinding(in2));
        }

        public FirstNotNullExtension(object in1, object in2, object in3) : this(in1, in2)
        {
            _multiBinding.Bindings.Add(convertBinding(in3));
        }

        public FirstNotNullExtension(object in1, object in2, object in3, object in4) : this(in1, in2, in3)
        {
            _multiBinding.Bindings.Add(convertBinding(in4));
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

        public string Debug
        {
            set
            {
                (_multiBinding.Converter as FirstNotNullConverter).Debug = value;
            }
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return _multiBinding.ProvideValue(serviceProvider);
        }

     }
}

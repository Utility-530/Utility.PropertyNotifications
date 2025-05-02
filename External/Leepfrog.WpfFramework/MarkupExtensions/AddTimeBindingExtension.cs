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
using static Leepfrog.WpfFramework.Converters.AddTimeConverter;

namespace Leepfrog.WpfFramework
{
    public class AddTimeBindingExtension :
            MarkupExtension
    {
        /// <summary>
        /// Markup extension that returns a multibinding to add a period time
        /// </summary>
        /// <remarks>
        /// </remarks>

        public AddTimeBindingExtension()
        {

        }

        private MultiBinding _multiBinding;

        public AddTimeBindingExtension(object date, object number, TimePeriods period)
        {
            _multiBinding = new MultiBinding();
            _multiBinding.Mode = BindingMode.OneWay;
            _multiBinding.Converter = new AddTimeConverter();
            _multiBinding.Bindings.Add(convertBinding(date));
            _multiBinding.Bindings.Add(convertBinding(number));
            _multiBinding.Bindings.Add(convertBinding(period));
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

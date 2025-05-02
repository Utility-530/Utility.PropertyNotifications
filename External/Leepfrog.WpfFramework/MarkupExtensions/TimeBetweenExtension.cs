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

namespace Leepfrog.WpfFramework
{
    public class TimeBetweenExtension :
            MarkupExtension
    {
        /// <summary>
        /// Markup extension that returns a multibinding to do add a number of days
        /// </summary>
        /// <remarks>
        /// </remarks>

        public TimeBetweenExtension()
        {

        }

        private MultiBinding _multiBinding;

        public TimeBetweenExtension(object date1, object date2)
        {
            _multiBinding = new MultiBinding();
            _multiBinding.Mode = BindingMode.OneWay;
            _multiBinding.Converter = new TimeBetweenConverter(false);
            _multiBinding.Bindings.Add(convertBinding(date1));
            _multiBinding.Bindings.Add(convertBinding(date2));
        }

        public TimeBetweenExtension(object date1, object date2, bool returnRaw)
        {
            _multiBinding = new MultiBinding();
            _multiBinding.Mode = BindingMode.OneWay;
            _multiBinding.Converter = new TimeBetweenConverter(returnRaw);
            _multiBinding.Bindings.Add(convertBinding(date1));
            _multiBinding.Bindings.Add(convertBinding(date2));
        }
        public TimeBetweenExtension(object date1, object date2, bool returnRaw, TimeSpan fallback)
        {
            _multiBinding = new MultiBinding();
            _multiBinding.Mode = BindingMode.OneWay;
            _multiBinding.Converter = new TimeBetweenConverter(returnRaw, fallback);
            _multiBinding.Bindings.Add(convertBinding(date1));
            _multiBinding.Bindings.Add(convertBinding(date2));
            _multiBinding.FallbackValue = fallback;
        }

        private BindingBase convertBinding(object param, IValueConverter conv = null)
        {
            Binding binding;
            if ( param is Binding )
            {
                binding = ( param as Binding );
            }
            else
            {
                binding = new Binding();
                binding.Source = param;
            }
            if ( binding.Converter == null )
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

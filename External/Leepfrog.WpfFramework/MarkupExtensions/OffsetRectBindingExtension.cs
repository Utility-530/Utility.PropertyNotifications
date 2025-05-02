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
    public class OffsetRectBindingExtension :
            MarkupExtension
    {
        /// <summary>
        /// Markup extension that returns a multibinding to do an IF then else
        /// </summary>
        /// <remarks>
        /// </remarks>

        public OffsetRectBindingExtension()
        {

        }

        private MultiBinding _multiBinding;

        public OffsetRectBindingExtension(object x1, object y1, object x2, object y2, object offsetX, object offsetY)
        {
            _multiBinding = new MultiBinding();
            _multiBinding.Mode = BindingMode.OneWay;
            _multiBinding.Converter = new OffsetRectConverter();
            _multiBinding.Bindings.Add(convertBinding(x1));
            _multiBinding.Bindings.Add(convertBinding(y1));
            _multiBinding.Bindings.Add(convertBinding(x2));
            _multiBinding.Bindings.Add(convertBinding(y2));
            _multiBinding.Bindings.Add(convertBinding(offsetX));
            _multiBinding.Bindings.Add(convertBinding(offsetY));
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

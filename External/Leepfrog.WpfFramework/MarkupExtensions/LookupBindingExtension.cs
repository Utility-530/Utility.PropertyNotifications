using System;
using System.Windows.Data;
using System.Windows.Markup;

namespace Leepfrog.WpfFramework.Converters
{
    public class LookupBindingExtension :
                MarkupExtension
    {
        /// <summary>
        /// Markup extension that returns a multibinding to do an lookup in a dictionary
        /// </summary>
        /// <remarks>
        /// </remarks>

        public LookupBindingExtension()
        {

        }

        private MultiBinding _multiBinding;

        public LookupBindingExtension(object collection, object index) : this(collection, index, null)
        { }

        public LookupBindingExtension(object collection, object index, object dummyLoader = null)
        {
            _multiBinding = new MultiBinding();
            _multiBinding.Mode = BindingMode.OneWay;
            _multiBinding.Converter = new Leepfrog.WpfFramework.Converters.LookupMultiConverter();
            _multiBinding.Bindings.Add(convertBinding(dummyLoader));
            _multiBinding.Bindings.Add(convertBinding(collection));
            _multiBinding.Bindings.Add(convertBinding(index));
        }

        public LookupBindingExtension(object collection, object index, object index2, object dummyLoader = null)
        {
            _multiBinding = new MultiBinding();
            _multiBinding.Mode = BindingMode.OneWay;
            _multiBinding.Converter = new Leepfrog.WpfFramework.Converters.LookupMultiConverter();
            _multiBinding.Bindings.Add(convertBinding(dummyLoader));
            _multiBinding.Bindings.Add(convertBinding(collection));
            _multiBinding.Bindings.Add(convertBinding(index));
            _multiBinding.Bindings.Add(convertBinding(index2));
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

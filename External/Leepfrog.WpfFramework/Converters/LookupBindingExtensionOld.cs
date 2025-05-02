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
    public class LookupBindingExtension :
            MarkupExtension
    {
        /// <summary>
        /// Markup extension that returns a multibinding to lookup x in y
        /// </summary>
        /// <remarks>
        /// LookupLinkConverter acts as a multivalue converter.
        /// It is also a markup extension which allows to avoid declaring resources
        /// </remarks>

        private BindingBase _dictionary;
        private BindingBase _key;
        private MultiBinding _multiBinding;

        public LookupBindingExtension()
        {

        }
        public LookupBindingExtension(BindingBase repository, BindingBase key)
        {
            _dictionary = repository;
            _key = key;
            _multiBinding = new MultiBinding();
            _multiBinding.Mode = BindingMode.OneWay;
            _multiBinding.Converter = new LookupLinkConverter();
            _multiBinding.Bindings.Add(_dictionary);
            _multiBinding.Bindings.Add(_key);
        }
        
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return _multiBinding;//.ProvideValue(serviceProvider);
        }

     }
}

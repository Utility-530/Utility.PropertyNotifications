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
    public class ConverterChainExtension :
            MarkupExtension
    {
        /// <summary>
        /// Markup extension that returns a converter chain
        /// </summary>
        /// <remarks>
        /// This is a markup extension which allows to avoid declaring resources
        /// </remarks>

        private ConverterGroup _converter;
        private IValueConverter[] _children;

        public ConverterChainExtension()
        {

        }
        public ConverterChainExtension(IValueConverter a, IValueConverter b)
        {
            var converters = new List<IValueConverter>();
            converters.Add(a);
            converters.Add(b);
            _children = converters.ToArray();
            _converter = new ConverterGroup();
            _converter.AddRange(_children);
        }
        
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return _converter;
        }
     }
}

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
    public class MatrixToScaleConverterExtension :
            MarkupExtension
    {
        /// <summary>
        /// Markup extension that returns a converter chain
        /// </summary>
        /// <remarks>
        /// This is a markup extension which allows to avoid declaring resources
        /// </remarks>

        private MatrixToScaleConverter _converter;

        public MatrixToScaleConverterExtension()
        {
            _converter = new MatrixToScaleConverter();
        }
        
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return _converter;
        }
     }
}

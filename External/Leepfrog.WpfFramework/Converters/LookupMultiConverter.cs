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
using System.Reflection;

namespace Leepfrog.WpfFramework.Converters
{
    public class LookupMultiConverter :
            MarkupExtension,
          IMultiValueConverter
    {
        /// <summary>
        /// Value converter that looks up the specified key in a dictionary or similar
        /// </summary>
        /// <remarks>
        /// LookupLinkConverter acts as a multivalue converter.
        /// It is also a markup extension which allows to avoid declaring resources
        /// </remarks>

        public LookupMultiConverter()
        {

        }

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            // 1st item is a dummy to force load the repository
            // 2nd is the repository
            // 3rd is the key
            // 4th (optional) is an additional key
            
            try
            {
                if (values.Length < 2)
                {
                    return DependencyProperty.UnsetValue;
                }
                object repository = values[1];
                if ((repository == null) || (repository == DependencyProperty.UnsetValue))
                {
                    return DependencyProperty.UnsetValue;
                }
                var types = values.Skip(2).Select(v => v.GetType()).ToArray();
                var data = values.Skip(2).ToArray();
                Type type = repository.GetType();
                PropertyInfo indexerProperty = type.GetProperty("Item", types);
                if (indexerProperty == null)
                {
                    return DependencyProperty.UnsetValue;
                }
                object value = indexerProperty.GetValue(repository, data);

                return value;// can't use dynamic to do this! homogenous appdomain exception?! repository[key];
            }
            catch
            {

            }
            return DependencyProperty.UnsetValue;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }

    }
}

using Leepfrog.WpfFramework.Triggers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Leepfrog.WpfFramework.Converters
{
    /// <summary>
    /// Helper class for showing a control when a binding exists
    /// </summary>
    public class ItemsSourceMultiConverter : IMultiValueConverter
    {

        public ItemsSourceMultiConverter()
        {
        }

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var trigger = parameter as ItemsSourceDataTrigger;
            if (trigger != null)
            {
                return trigger.Evaluate(values);
            }
            return Binding.DoNothing;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return (object[])value;
        }
    }


}

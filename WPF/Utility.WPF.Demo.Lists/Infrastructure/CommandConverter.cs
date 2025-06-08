using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using Utility.WPF.Controls.Lists;
using Utility.WPF.Models;

namespace Utility.WPF.Demo.Lists.Infrastructure
{
    internal class CommandConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is CheckedRoutedEventArgs args)
            {
                if(args.OriginalSource is CheckBoxesListControl c)
                {
                    foreach(var item in c.ItemsSource)
                    {

                    }
                }
            }
            return new Dictionary<object, bool?>();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

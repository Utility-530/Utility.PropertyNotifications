using Splat;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reactive.Threading.Tasks;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using Utility.Interfaces.Exs;
using Utility.Models.Trees;

namespace Utility.Nodes.Demo.Transformers
{
    public class DirtyConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is RoutedPropertyChangedEventArgs<object> { NewValue: NodeViewModel { Data: DirtyModel { SourceKey: { } sourceKey } } newValue } args)
            {
                var x = Locator.Current.GetService<INodeSource>();
                var single = x.Single(sourceKey).ToTask().Result;
                return single;
            }
  
            return DependencyProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

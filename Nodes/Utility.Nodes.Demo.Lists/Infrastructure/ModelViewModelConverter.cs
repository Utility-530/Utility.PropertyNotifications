using System.Globalization;
using System.Windows;
using System.Windows.Data;
using Utility.Nodes.Demo.Lists.Infrastructure;
using Utility.Nodes.Demo.Lists.Services;

namespace Utility.Nodes.Demo.Lists
{
    public class ModelViewModelConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value switch
            {
                EbayModel model => new TreeViewModel(nameof(NodeMethodFactory.BuildEbayRoot), model.Id, model),
                null => DependencyProperty.UnsetValue,
                _ => throw new NotImplementedException(),
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

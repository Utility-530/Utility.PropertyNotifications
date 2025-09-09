using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using Utility.Trees;

namespace Utility.Nodify.Views.Infrastructure
{
    public class NodeEventArgsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is WPF.Controls.ComboBoxes.ComboBoxTreeView.SelectedNodeEventArgs { Value: Tree x } args)
            {
                return x.Data;
            }
            throw new NotImplementedException("SD");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

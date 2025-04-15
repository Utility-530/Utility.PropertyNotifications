using System.Globalization;
using System.Windows;
//using static Controls.Infrastructure.ItemsPanelFactory;
using Orientation = Utility.Enums.Orientation;
using O = System.Windows.Controls.Orientation;
using A = Utility.Enums.Arrangement;
using Arrangement = Utility.Enums.Arrangement;
using System;
using Utility.WPF.Factorys;

namespace Utility.WPF.Controls.Trees
{
    //public class ItemsPanelConverter : System.Windows.Data.IValueConverter
    //{
    //    public static ItemsPanelConverter Instance { get; } = new();

    //    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    //    {
    //        if (value is IPanelArranger { Arrangement: { } arrangement, Rows: var rows, Columns: var columns, Orientation: var orientation })
    //        {
    //            if (orientation == Orientation.Vertical)
    //            {

    //            }
    //            var template = ItemsPanelFactory.Template(
    //                rows,
    //                columns,
    //                (O)Enum.Parse(typeof(O), orientation.ToString()),
    //                (A)Enum.Parse(typeof(A), arrangement.ToString()));
    //            return template;
    //        }

    //        return DependencyProperty.UnsetValue;
    //    }

    //    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    //    {
    //        throw new NotImplementedException();
    //    }
    //}

    public class MultiItemsPanelConverter : System.Windows.Data.IMultiValueConverter
    {
        public static MultiItemsPanelConverter Instance { get; } = new();

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values is object[] { } array)
            {
                O o;
                if (array[3] is Orientation orientation)
                {
                    o = Enum.TryParse(typeof(O), orientation.ToString(), out var obj) ? (O)obj : O.Horizontal;
                }
                else
                {
                    o = (O)array[3];
                }
                if (
                    array[0] is Arrangement arrangement &&
                    array[1] is var rows &&
                    array[2] is var columns)
                {
                    var template = ItemsPanelFactory.Template(
                        (int?)rows,
                        (int?)columns,
                        o,
                        (A)Enum.Parse(typeof(A), arrangement.ToString()));
                    return template;
                }
            }
            return DependencyProperty.UnsetValue;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class VisibilityConverter : System.Windows.Data.IValueConverter
    {
        public static VisibilityConverter Instance { get; } = new();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool?)
            {
                switch ((bool?)value)
                {
                    case null:
                        return Visibility.Hidden;
                    case false:
                        return Visibility.Collapsed;
                    case true:
                        return Visibility.Visible;
                }
            }

            return DependencyProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class InverseConverter : System.Windows.Data.IValueConverter
    {
        public static InverseConverter Instance { get; } = new();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool?)
            {
                switch ((bool?)value)
                {
                    case null:
                        return null;
                    case false:
                        return true;
                    case true:
                        return false;
                }
            }

            return DependencyProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool?)
            {
                switch ((bool?)value)
                {
                    case null:
                        return null;
                    case false:
                        return true;
                    case true:
                        return false;
                }
            }

            return DependencyProperty.UnsetValue;

        }
    }
}

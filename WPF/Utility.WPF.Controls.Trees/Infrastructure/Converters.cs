using System.Globalization;
using System.Windows;
//using static Controls.Infrastructure.ItemsPanelFactory;
using Orientation = Utility.Enums.Orientation;
using O = System.Windows.Controls.Orientation;
using Arrangement = Utility.Enums.Arrangement;
using System;
using Utility.WPF.Factorys;
using Utility.Structs;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;

namespace Utility.WPF.Controls.Trees
{
    public class MultiItemsPanelConverter : System.Windows.Data.IMultiValueConverter
    {
        public static MultiItemsPanelConverter Instance { get; } = new();

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values is object[] { } array)
            {
                Arrangement? arrangement = default;
                if (array.SingleOrDefault(x => x is string) is string itemsPanelTemplate)
                {
                    if (Enum.TryParse<Arrangement>(itemsPanelTemplate, out var result))
                    {
                        arrangement = result;
                    }
                    else if (Application.Current.Resources[itemsPanelTemplate] is ItemsPanelTemplate template)
                        return template;
                }

                if (arrangement.HasValue == false && array.SingleOrDefault(x => x is Arrangement) is Arrangement arr)
                {
                    arrangement = arr;
                }


                O? o = default;
                if (array[4] is Orientation orientation)
                {
                    o = Enum.TryParse(typeof(O), orientation.ToString(), out var obj) ? (O)obj : O.Horizontal;
                }
                else if (array[4] != DependencyProperty.UnsetValue)
                {
                    o = (O)array[4];
                }
                if (
                    arrangement.HasValue &&
                    array[2] is var rows &&
                    array[3] is var columns)
                {
                    var template = ItemsPanelFactory.Template(
                        (IReadOnlyCollection<Dimension>?)rows,
                        (IReadOnlyCollection<Dimension>?)columns,
                        o,
                        arrangement);
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

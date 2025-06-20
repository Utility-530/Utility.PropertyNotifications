using System.Globalization;
using System.Windows;
//using static Controls.Infrastructure.ItemsPanelFactory;
using Orientation = Utility.Enums.Orientation;
using O = System.Windows.Controls.Orientation;
using A = Utility.Enums.Arrangement;
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
                if(array.SingleOrDefault(x=>x is string) is { } itemsPanelTemplate)
                {
                    if (Application.Current.Resources[itemsPanelTemplate] is ItemsPanelTemplate template)
                        return template;
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
                    array[1] is Arrangement arrangement &&
                    array[2] is var rows &&
                    array[3] is var columns)
                {
                    var template = ItemsPanelFactory.Template(
                        (IReadOnlyCollection<Dimension>?)rows,
                        (IReadOnlyCollection<Dimension>?)columns,
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

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
using System.Windows.Data;

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
                O? orientation = default;
                IReadOnlyCollection<Dimension>? rows = default;
                IReadOnlyCollection<Dimension>? columns = default;

                foreach (var x in array)
                {
                    if (x is string s)
                    {
                        if (Enum.TryParse<Arrangement>(s, out var result))
                        {
                            arrangement = result;
                        }
                        else if (Enum.TryParse<O>(s, out var resulto))
                        {
                            orientation = resulto;
                        }
                        else if (Application.Current.Resources[x] is ItemsPanelTemplate template)
                            return template;
                    }
                    else if (x is RowColumnConverter.RowColumns rc)
                    {
                        rows = rc.Rows;
                        columns = rc.Columns;
                    }
                    else if (x is Arrangement arr)
                    {
                        arrangement = arr;
                    }
                    else if (x is Orientation _o)
                    {
                        orientation = Enum.Parse<O>(_o.ToString());
                    }
                    else if (x is O o)
                    {
                        orientation = o;
                    }
                }


                return ItemsPanelFactory.Template(
                    rows,
                    columns,
                    orientation.HasValue ? orientation.Value : O.Vertical,
                    arrangement);

            }
            return DependencyProperty.UnsetValue;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class RowColumnConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            return new RowColumns(values[0] as IReadOnlyCollection<Dimension>, values[1] as IReadOnlyCollection<Dimension>);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public record RowColumns(IReadOnlyCollection<Dimension> Rows, IReadOnlyCollection<Dimension> Columns);
    }

}

using System.Collections;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Utility.Structs;

namespace Utility.WPF.Panels
{
    /// <summary>
    /// ALlows setting the row heights/ column-widths via binding
    /// </summary>
    public class GridEx : Grid
    {
        public static readonly DependencyProperty RowHeightsProperty =
            DependencyProperty.Register("RowHeights", typeof(IEnumerable), typeof(GridEx), new PropertyMetadata(rowsChanged));

        public static readonly DependencyProperty ColumnWidthsProperty =
            DependencyProperty.Register("ColumnWidths", typeof(IEnumerable), typeof(GridEx), new PropertyMetadata(columnsChanged));

        public IEnumerable RowHeights
        {
            get { return (IEnumerable)GetValue(RowHeightsProperty); }
            set { SetValue(RowHeightsProperty, value); }
        }

        private static void rowsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is GridEx g && e.NewValue is IEnumerable<Dimension> dimensions)
            {
                foreach (var dimension in dimensions)
                {
                    g.RowDefinitions.Add(new RowDefinition { Height = new GridLength(dimension.Value, (GridUnitType)dimension.UnitType) });
                }
            }
        }

        public IEnumerable ColumnWidths
        {
            get { return (IEnumerable)GetValue(ColumnWidthsProperty); }
            set { SetValue(ColumnWidthsProperty, value); }
        }

        private static void columnsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is GridEx g && e.NewValue is IEnumerable<Dimension> dimensions)
            {
                foreach (var dimension in dimensions)
                {
                    g.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(dimension.Value, (GridUnitType)dimension.UnitType) });
                }
            }
        }
    }
}
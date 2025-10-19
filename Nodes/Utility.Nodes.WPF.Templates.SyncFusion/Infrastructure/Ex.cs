using Syncfusion.UI.Xaml.Charts;
using Syncfusion.UI.Xaml.Grid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Utility.Nodes.WPF.Templates.SyncFusion
{
    public static class Ex
    {
        public static readonly DependencyProperty AutoColumnsProperty =
            DependencyProperty.RegisterAttached(
                "AutoColumns",
                typeof(IEnumerable<GridColumn>),
                typeof(Ex),
                new PropertyMetadata(null, OnAutoColumnsChanged));

        public static void SetAutoColumns(DependencyObject element, IEnumerable<GridColumn> value)
            => element.SetValue(AutoColumnsProperty, value);

        public static IEnumerable<GridColumn> GetAutoColumns(DependencyObject element)
            => (IEnumerable<GridColumn>)element.GetValue(AutoColumnsProperty);

        private static void OnAutoColumnsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is SfDataGrid grid)
            {
                grid.Columns.Clear();
                if (e.NewValue is IEnumerable<GridColumn> cols)
                {
                    foreach (var c in cols)
                        grid.Columns.Add(c);
                }
            }
        }


        // XAxisProperty
        public static readonly DependencyProperty XAxisProperty =
            DependencyProperty.RegisterAttached(
                "XAxis",
                typeof(string),
                typeof(Ex),
                new PropertyMetadata(null, OnAxisChanged));

        public static string GetXAxis(DependencyObject obj) => (string)obj.GetValue(XAxisProperty);
        public static void SetXAxis(DependencyObject obj, string value) => obj.SetValue(XAxisProperty, value);

        // YAxisProperty
        public static readonly DependencyProperty YAxisProperty =
            DependencyProperty.RegisterAttached(
                "YAxis",
                typeof(string),
                typeof(Ex),
                new PropertyMetadata(null, OnYAxisChanged));

        public static string GetYAxis(DependencyObject obj) => (string)obj.GetValue(YAxisProperty);
        public static void SetYAxis(DependencyObject obj, string value) => obj.SetValue(YAxisProperty, value);

        // Common callback for either axis
        private static void OnAxisChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is SfDateTimeRangeNavigator nav)
            {
                string? xPath = GetXAxis(nav);
                string? yPath = GetYAxis(nav);

                // Apply XBindingPath
                if (!string.IsNullOrWhiteSpace(xPath))
                    nav.XBindingPath = xPath;          
            }
            else if (d is SfLineSparkline nav2)
            {
                string? xPath = GetXAxis(nav2);

                nav2.XBindingPath = xPath;
            }
            else if(d is ChartSeriesBase nav3)
            {
                string? xPath = GetXAxis(nav3);

                nav3.XBindingPath = xPath;
            }
            else
            {

            }
        }
        private static void OnYAxisChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is SfLineSparkline nav2)
            {         
                string? yPath = GetYAxis(nav2);

                nav2.YBindingPath = yPath;
            }
            else if(d is SplineSeries nav3)
            {
                string? yPath = GetYAxis(nav3);

                nav3.YBindingPath = yPath;
            }
            else if(d is XyDataSeries nav4)
            {
                string? yPath = GetYAxis(nav4);

                nav4.YBindingPath = yPath;
            }
            else
            {

            }

        }

        // Helper to find sparkline in content
        private static SfLineSparkline? FindChildSparkline(FrameworkElement parent)
        {
            if (parent is SfLineSparkline s)
                return s;

            if (parent is Panel p)
            {
                foreach (var child in p.Children)
                {
                    if (child is FrameworkElement fe)
                    {
                        var found = FindChildSparkline(fe);
                        if (found != null)
                            return found;
                    }
                }
            }

            return null;
        }
    }
}


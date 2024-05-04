using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
//using PixelLab.Wpf;
using Utility.Enums;
using Orientation = Utility.Enums.Orientation;
//using static Controls.Infrastructure.TemplateGenerator;
using static Utility.WPF.Factorys.TemplateGenerator;

namespace Controls.Infrastructure
{
    public static class ItemsPanelFactory
    {
        public static ItemsPanelTemplate Template(int? rows, int? columns, Orientation? orientation, Arrangement? arrangement)
        {

            return (arrangement, orientation) switch
            {

                //(Arrangement.Grid, _) => CreateItemsPanelTemplate<AutoGrid>(factory =>
                //{
                //    if (rows.HasValue)
                //    {
                //        factory.SetValue(AutoGrid.RowsCountProperty, rows.Value);
                //    }
                //    if (columns.HasValue)
                //    {
                //        factory.SetValue(AutoGrid.ColumnsCountProperty, columns.Value);
                //    }
                //    if (orientation.HasValue)
                //        factory.SetValue(AutoGrid.OrientationProperty, orientation.Value);
                //}),
                (Arrangement.Stacked, _) =>
                CreateItemsPanelTemplate<StackPanel>(SetStackPanelOrientation),
                (Arrangement.Wrapped, _) =>
                CreateItemsPanelTemplate<WrapPanel>(SetWrapPanelOrientation),
                //(Arrangement.Row, _) =>
                //CreateItemsPanelTemplate<UniformGrid>(factory =>
                //{
                //    factory.SetValue(UniformGrid.ColumnsProperty, columns ?? 2);
                //    factory.SetValue(UniformGrid.RowsProperty, rows ?? 1);
                //}),
                //(Arrangement.Column, _) =>
                //CreateItemsPanelTemplate<UniformGrid>(factory =>
                //{
                //    factory.SetValue(UniformGrid.ColumnsProperty, columns ?? 1);
                //    factory.SetValue(UniformGrid.RowsProperty, rows ?? 2);
                //}),

                (_, Orientation.Vertical) =>
                CreateItemsPanelTemplate<UniformGrid>(factory =>
                {
                    factory.SetValue(UniformGrid.ColumnsProperty, columns ?? 1);
                    factory.SetValue(UniformGrid.RowsProperty, rows ?? 2);
                }),

                (_, Orientation.Horizontal) =>
                CreateItemsPanelTemplate<UniformGrid>(factory =>
                {
                    factory.SetValue(UniformGrid.ColumnsProperty, columns ?? 2);
                    factory.SetValue(UniformGrid.RowsProperty, rows ?? 1);
                }),
                (Arrangement.Uniform, _) =>
                CreateItemsPanelTemplate<UniformGrid>(factory =>
                {
                    factory.SetValue(UniformGrid.RowsProperty, rows ?? 2);
                    factory.SetValue(UniformGrid.ColumnsProperty, columns ?? 2);
                }),

                //(Arrangement.TreeMap, _) => CreateItemsPanelTemplate<TreeMapPanel>(),
                //(Arrangement.TreeStack, _) => CreateItemsPanelTemplate<TreeStackPanel>(),

                _ => throw new Exception("WGE vgfd vvf")
            };

            void SetStackPanelOrientation(FrameworkElementFactory factory)
            {
                orientation ??= (Orientation)WrapPanel.OrientationProperty.DefaultMetadata.DefaultValue;
                factory.SetValue(StackPanel.OrientationProperty, orientation.Value);
            }
            void SetWrapPanelOrientation(FrameworkElementFactory factory)
            {
                orientation ??= (Orientation)WrapPanel.OrientationProperty.DefaultMetadata.DefaultValue;
                factory.SetValue(WrapPanel.OrientationProperty, orientation.Value);
            }
            //void SetTreeMapOrientation(FrameworkElementFactory factory)
            //{
            //    factory.SetValue(WrapPanel.OrientationProperty, orientation);
            //}
            //void SetTreeStackOrientation(FrameworkElementFactory factory)
            //{
            //    factory.SetValue(WrapPanel.OrientationProperty, orientation);
            //}
        }
    }
}


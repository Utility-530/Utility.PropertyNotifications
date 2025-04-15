using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Utility.Enums;
using Utility.WPF.Panels;
using PixelLab.Wpf;

namespace Utility.WPF.Factorys
{
    using static TemplateGenerator;
    using Orientation = System.Windows.Controls.Orientation;

    public static class ItemsPanelFactory
    {
        public static ItemsPanelTemplate Template(int? rows, int? columns, Orientation? orientation, Arrangement? arrangement, Action<FrameworkElementFactory>? postAction =null)
        {
            //if(rows==1 && orientation==Orientation.Horizontal && arrangement == Arrangement.Uniform)
            //{
            //    return CreateItemsPanelTemplate<UniformStackPanel>(factory =>
            //    {
            //        factory.SetValue(UniformStackPanel.OrientationProperty, Orientation.Horizontal);
            //        postAction?.Invoke(factory);
            //    });
            //}
            //if (columns == 1 && orientation == Orientation.Vertical && arrangement == Arrangement.Uniform)
            //{
            //    return CreateItemsPanelTemplate<UniformStackPanel>(factory =>
            //    {
            //        factory.SetValue(UniformStackPanel.OrientationProperty, Orientation.Vertical);
            //        postAction?.Invoke(factory);
            //    });
            //}
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
                (Arrangement.Stack, _) =>
                CreateItemsPanelTemplate<StackPanel>(SetStackPanelOrientation),
                (Arrangement.Wrap, _) =>
                CreateItemsPanelTemplate<WrapPanel>(SetWrapPanelOrientation),
                //(Arrangement.Horizotal, _) =>
                //CreateItemsPanelTemplate<UniformGrid>(factory =>
                //{
                //    factory.SetValue(UniformGrid.ColumnsProperty, columns ?? 2);
                //    factory.SetValue(UniformGrid.RowsProperty, rows ?? 1);
                //}),
                //(Arrangement.Vertical, _) =>
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
                    postAction?.Invoke(factory);
                }),

                (_, Orientation.Horizontal) =>
                CreateItemsPanelTemplate<UniformGrid>(factory =>
                {
                    factory.SetValue(UniformGrid.ColumnsProperty, columns ?? 2);
                    factory.SetValue(UniformGrid.RowsProperty, rows ?? 1);
                    postAction?.Invoke(factory);
                }),
                (Arrangement.Uniform, _) =>
                CreateItemsPanelTemplate<UniformGrid>(factory =>
                {
                    factory.SetValue(UniformGrid.RowsProperty, rows ?? 2);
                    factory.SetValue(UniformGrid.ColumnsProperty, columns ?? 2);
                    postAction?.Invoke(factory);

                }),

                (Arrangement.TreeMap, _) => CreateItemsPanelTemplate<TreeMapPanel>(),
                //(Arrangement.TreeStack, _) => CreateItemsPanelTemplate<TreeStackPanel>(),

                _ => throw new Exception("WGE vgfd vvf")
            };

            void SetStackPanelOrientation(FrameworkElementFactory factory)
            {
                orientation ??= (Orientation)WrapPanel.OrientationProperty.DefaultMetadata.DefaultValue;
                factory.SetValue(StackPanel.OrientationProperty, orientation.Value);
                postAction?.Invoke(factory);
            }
            void SetWrapPanelOrientation(FrameworkElementFactory factory)
            {
                orientation ??= (Orientation)WrapPanel.OrientationProperty.DefaultMetadata.DefaultValue;
                factory.SetValue(WrapPanel.OrientationProperty, orientation.Value);
                postAction?.Invoke(factory);

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


using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using PixelLab.Wpf;
using Utility.Enums;
using Utility.Structs;
using Utility.WPF.Panels;

namespace Utility.WPF.Factorys
{
    using static TemplateGenerator;
    using Orientation = System.Windows.Controls.Orientation;

    public static class ItemsPanelFactory
    {
        public static ItemsPanelTemplate Template(IReadOnlyCollection<Dimension>? rows, IReadOnlyCollection<Dimension>? columns, Orientation? orientation, Arrangement? arrangement, Action<FrameworkElementFactory>? postAction = null)
        {
            return (arrangement, orientation) switch
            {
                (Arrangement.Grid, _) =>
                                CreateItemsPanelTemplate<GridEx>(factory =>
                                {
                                    factory.SetValue(GridEx.RowHeightsProperty, rows);
                                    factory.SetValue(GridEx.ColumnWidthsProperty, columns);
                                    postAction?.Invoke(factory);
                                }),

                (Arrangement.Stack, _) =>
                CreateItemsPanelTemplate<StackPanel>(SetStackPanelOrientation),
                (Arrangement.Wrap, _) =>
                CreateItemsPanelTemplate<WrapPanel>(SetWrapPanelOrientation),
                (Arrangement.Uniform, _) =>
                CreateItemsPanelTemplate<UniformGrid>(factory =>
                {
                    factory.SetValue(UniformGrid.RowsProperty, rows?.Count ?? 2);
                    factory.SetValue(UniformGrid.ColumnsProperty, columns?.Count ?? 2);
                    postAction?.Invoke(factory);
                }),

                (Arrangement.TreeMap, _) => CreateItemsPanelTemplate<TreeMapPanel>(),
                (Arrangement.MasterSlave, Orientation.Horizontal) => CreateItemsPanelTemplate<MasterSlavePanel>(a => a.SetValue(MasterSlavePanel.MasterPositionProperty, Dock.Left)),
                (Arrangement.MasterSlave, Orientation.Vertical) => CreateItemsPanelTemplate<MasterSlavePanel>(a=> a.SetValue(MasterSlavePanel.MasterPositionProperty, Dock.Top)),
                //(Arrangement.TreeStack, _) => CreateItemsPanelTemplate<TreeStackPanel>(),
                (_, Orientation.Vertical) =>
                CreateItemsPanelTemplate<UniformGrid>(factory =>
                {
                    factory.SetValue(UniformGrid.ColumnsProperty, columns?.Count ?? 1);
                    factory.SetValue(UniformGrid.RowsProperty, rows?.Count ?? 2);
                    postAction?.Invoke(factory);
                }),

                (_, Orientation.Horizontal) =>
                CreateItemsPanelTemplate<UniformGrid>(factory =>
                {
                    factory.SetValue(UniformGrid.ColumnsProperty, columns?.Count ?? 2);
                    factory.SetValue(UniformGrid.RowsProperty, rows?.Count ?? 1);
                    postAction?.Invoke(factory);
                }),
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
        }
    }
}
using System;
using System.Windows.Controls.Primitives;
using System.Windows.Controls;
using System.Windows;
using static Utility.WPF.Helpers.TemplateGenerator;
using Arrangement = Utility.Enums.Arrangement;

namespace Utility.WPF.Helpers
{
    public class LayOutHelper
    {
        //public enum Arrangement
        //{
        //    Stacked, Wrapped, Uniform, Custom
        //}

        public static void Changed(ItemsControl itemsControl, Orientation orientation, Arrangement arrangement)
        {
            itemsControl.ItemsPanel = ItemsPanelTemplate(itemsControl.Items.Count, orientation, arrangement);
        }

        public static ItemsPanelTemplate ItemsPanelTemplate(int count, Orientation orientation, Arrangement arrangement)
        {
            return (arrangement, orientation) switch
            {
                (Arrangement.Stacked,_) =>
                CreateItemsPanelTemplate<StackPanel>(SetStackPanelOrientation),
                (Arrangement.Wrapped, _) =>
                CreateItemsPanelTemplate<WrapPanel>(SetWrapPanelOrientation), 
                (Arrangement.Uniform, Orientation.Vertical) =>
                CreateItemsPanelTemplate<UniformGrid>(factory =>
                {
                    factory.SetValue(UniformGrid.ColumnsProperty, 1);
                    factory.SetValue(UniformGrid.RowsProperty, count);
                }),
                (Arrangement.Uniform, Orientation.Horizontal) =>
                CreateItemsPanelTemplate<UniformGrid>(factory =>
                {
                    factory.SetValue(UniformGrid.RowsProperty, 1);
                    factory.SetValue(UniformGrid.ColumnsProperty, count);
                }),
                _ => throw new Exception("WGE vgfd vvf")
            };

            void SetStackPanelOrientation(FrameworkElementFactory factory)
            {
                factory.SetValue(StackPanel.OrientationProperty, orientation);
            }
            void SetWrapPanelOrientation(FrameworkElementFactory factory)
            {
                factory.SetValue(WrapPanel.OrientationProperty, orientation);
            }
        }
    }
}

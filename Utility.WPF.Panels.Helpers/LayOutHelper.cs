using System;
using System.Windows;
using System.Windows.Controls;
using static Utility.WPF.Helpers.TemplateGenerator;
using Arrangement = Utility.Enums.Arrangement;
using Orientation = System.Windows.Controls.Orientation;

namespace Utility.WPF.Panels.Helpers
{
    public class LayOutHelper
    {
        public static void Changed(ItemsControl itemsControl, Orientation orientation, Arrangement arrangement)
        {
            itemsControl.ItemsPanel = ItemsPanelTemplate();

            ItemsPanelTemplate ItemsPanelTemplate()
            {
                return (arrangement, orientation) switch
                {
                    (Arrangement.Stacked, _) => BasicItemsPanelTemplate(),
                    (Arrangement.Wrapped, _) => BasicItemsPanelTemplate(),
                    (Arrangement.Uniform, _) => BasicItemsPanelTemplate(),
                    (Arrangement.Custom, _) => CreateItemsPanelTemplate<ArrangePanel>(SetArrangePanelOrientation),
                    _ => throw new Exception("WGE vgfd vvf")
                };
            }

            void SetArrangePanelOrientation(FrameworkElementFactory factory)
            {
                factory.SetValue(ArrangePanel.OrientationProperty, orientation);
            }

            ItemsPanelTemplate BasicItemsPanelTemplate()
            {
                return Utility.WPF.Helpers.LayOutHelper.ItemsPanelTemplate(itemsControl.Items.Count, orientation, arrangement);
            }
        }
    }
}
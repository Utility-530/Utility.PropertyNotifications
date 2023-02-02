using System;
using System.Windows.Controls.Primitives;
using System.Windows.Controls;
using System.Windows;
using static Utility.WPF.Helper.TemplateGenerator;
using arr = Utility.WPF.Helper.LayOutHelper.Arrangement;
using UtilityWpf.Panels;

using Orientation = System.Windows.Controls.Orientation;
using Arrangement = Utility.Enums.Arrangement;

namespace Utility.WPF.Panels.Helper
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
                return Utility.WPF.Helper.LayOutHelper.ItemsPanelTemplate(itemsControl.Items.Count, orientation, Map(arrangement));

                arr Map(Arrangement arrangement)
                {
                    return arrangement switch
                    {
                        Arrangement.Wrapped => arr.Wrapped,
                        Arrangement.Stacked => arr.Stacked,
                        Arrangement.Uniform => arr.Uniform,
                        _ => throw new Exception("dfv23c cfdf3II")
                    };
                }
            }
        }
    }
}

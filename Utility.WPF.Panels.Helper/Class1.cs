//using System;
//using System.Windows.Controls.Primitives;
//using System.Windows.Controls;
//using Utility.Common.Enum;
//using System.Windows;
//using static Utility.WPF.Helper.TemplateGenerator;

//namespace Utility.WPF.Panels.Helper
//{
//    public class LayOutHelper
//    {
//        public static void Changed(ItemsControl itemsControl, Orientation orientation, Arrangement arrangement)
//        {
//            itemsControl.ItemsPanel = ItemsPanelTemplate();

//            ItemsPanelTemplate ItemsPanelTemplate()
//            {
//                return (arrangement, orientation) switch
//                {
                  

//                    _ => throw new Exception("WGE vgfd vvf")
//                };
//            }

//            void SetStackPanelOrientation(FrameworkElementFactory factory)
//            {
//                factory.SetValue(StackPanel.OrientationProperty, orientation);
//            }
//            void SetWrapPanelOrientation(FrameworkElementFactory factory)
//            {
//                factory.SetValue(WrapPanel.OrientationProperty, orientation);
//            }
//        }
//    }
//}

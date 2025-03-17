using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows;

namespace Utility.WPF.Helpers
{
    public static class HitTestHelper
    {
        public static T? GetSelectedItem<T>(UIElement? sender, UIElement objTreeViewControl) where T : DependencyObject
        {
            Point? point = sender?.TranslatePoint(new Point(0, 0), objTreeViewControl);
            if (point.HasValue)
            {
                var hit = objTreeViewControl.InputHitTest(point.Value) as DependencyObject;
                while (hit is not null and not T)
                {
                    hit = VisualTreeHelper.GetParent(hit);
                    if (objTreeViewControl == hit)
                    {
                        hit = null;
                        break;
                    }
                }
                return (T?)hit;
            }
            return null;
        }
    }
}

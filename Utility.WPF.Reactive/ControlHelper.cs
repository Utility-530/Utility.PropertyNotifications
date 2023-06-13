using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Utility.WPF.Reactive
{
    public static class ControlHelper
    {
        /// <summary>
        /// <a href="https://social.msdn.microsoft.com/Forums/vstudio/en-US/c01df033-0acd-4690-a24d-3b7090694bc0/how-can-handle-treeviewitems-mouseclick-or-mousedoubleclick-event?forum=wpf"></a>
        /// Gets the top most item when the mouse double click returns multiple items, like with a TreeView
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static IObservable<DependencyObject?> MouseDoubleClickSelections(this Control control)
        {
            return Observable.FromEventPattern<MouseButtonEventHandler, MouseButtonEventArgs>(
                   s => control.MouseDoubleClick += s,
                   s => control.MouseDoubleClick -= s)
                .Select(evt =>
                {
                    return GetSelectedItem(evt.EventArgs.OriginalSource as UIElement, control);
                });

            DependencyObject? GetSelectedItem(UIElement? sender, UIElement objTreeViewControl)
            {
                Point? point = sender?.TranslatePoint(new Point(0, 0), objTreeViewControl);
                if (point .HasValue)
                {
                    var hit = objTreeViewControl.InputHitTest(point.Value) as DependencyObject;
                    while (hit is not null and not TreeViewItem)
                    {
                        hit = VisualTreeHelper.GetParent(hit);
                    }
                    return hit;
                }
                return null;
            }
        }
    }
}

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
using static Utility.WPF.Helpers.HitTestHelper;

namespace Utility.WPF.Reactives
{
    public static partial class ControlHelper
    {
        /// <summary>
        /// <a href="https://social.msdn.microsoft.com/Forums/vstudio/en-US/c01df033-0acd-4690-a24d-3b7090694bc0/how-can-handle-treeviewitems-mouseclick-or-mousedoubleclick-event?forum=wpf"></a>
        /// Gets the top most item when the mouse double click returns multiple items, like with a TreeView
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static IObservable<T?> MouseDoubleClickSelections<T>(this Control control) where T:DependencyObject
        {
            return Observable.FromEventPattern<MouseButtonEventHandler, MouseButtonEventArgs>(
                   s => control.MouseDoubleClick += s,
                   s => control.MouseDoubleClick -= s)
                .Select(evt =>
                {
                    return GetSelectedItem<T>(evt.EventArgs.OriginalSource as UIElement, control);
                });
        }

        public static IObservable<T?> MouseSingleClickSelections<T>(this UIElement control) where T : DependencyObject
        {
            return Observable.FromEventPattern<MouseButtonEventHandler, MouseButtonEventArgs>(
                   s => control.PreviewMouseLeftButtonUp += s,
                   s => control.PreviewMouseLeftButtonUp -= s)
                .Select(evt =>
                {
                    return GetSelectedItem<T>(evt.EventArgs.OriginalSource as UIElement, control);
                });
        }


        public static IObservable<T?> MouseHoverEnterSelections<T>(this UIElement control) where T : DependencyObject
        {
            return Observable.FromEventPattern<MouseEventHandler, MouseEventArgs>(
                   s => control.MouseEnter += s,
                   s => control.MouseEnter -= s)
                .Select(evt =>
                {
                    return GetSelectedItem<T>(evt.EventArgs.OriginalSource as UIElement, control);
                });
        }

        public static IObservable<(T? item, Point point)> MouseMoveSelections<T>(this UIElement control) where T : DependencyObject
        {
            return Observable.FromEventPattern<MouseEventHandler, MouseEventArgs>(
                   s => control.MouseMove += s,
                   s => control.MouseMove -= s)
                .Select(evt =>
                {
                    var point = evt.EventArgs.GetPosition(evt.EventArgs.OriginalSource as IInputElement);
                    //Point? point = control?.TranslatePoint(new Point(0, 0), objTreeViewControl);
                    return (GetSelectedItem<T>(evt.EventArgs.OriginalSource as UIElement, control), point);
                });
        }


        public static IObservable<(T? item, MouseEventArgs args)> MouseHoverLeaveSelections<T>(this UIElement control) where T : DependencyObject
        {
            return Observable.FromEventPattern<MouseEventHandler, MouseEventArgs>(
                   s => control.MouseLeave += s,
                   s => control.MouseLeave -= s)
                .Select(evt =>
                { 
                    return (GetSelectedItem<T>(evt.EventArgs.OriginalSource as UIElement, control), evt.EventArgs);
                });
        }
    }
}

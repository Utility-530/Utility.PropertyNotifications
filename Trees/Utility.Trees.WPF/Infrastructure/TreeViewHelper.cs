using System;
using System.Windows;
using System.Windows.Controls;
using Utility.WPF.Nodes;

namespace Utility.WPF.Nodes
{
    internal static class TreeViewHelper
    {
        /// <summary>
        /// <a href="https://social.msdn.microsoft.com/Forums/vstudio/en-US/c01df033-0acd-4690-a24d-3b7090694bc0/how-can-handle-treeviewitems-mouseclick-or-mousedoubleclick-event?forum=wpf"></a>
        /// Gets the top most item when the mouse double click returns multiple items, like with a TreeView
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static IObservable<HeaderedItemsControl?> MouseDoubleClicks(this ItemsControl control)
        {
            return control
                    .MouseDoubleClickSelections<HeaderedItemsControl>();
        }

        public static IObservable<HeaderedItemsControl?> MouseSingleClicks(this ItemsControl control)
        {
            return control
                    .MouseSingleClickSelections<HeaderedItemsControl>();
        }

        public static IObservable<HeaderedItemsControl> MouseHoverEnters(this ItemsControl control)
        {
            return control
                    .MouseHoverEnterSelections<HeaderedItemsControl>();
        }

        public static IObservable<HeaderedItemsControl> MouseHoverLeaves(this ItemsControl control)
        {
            return control
                    .MouseHoverLeaveSelections<HeaderedItemsControl>();
        }

        public static IObservable<(HeaderedItemsControl item, Point point)> MouseMoves(this ItemsControl control)
        {
            return control
                    .MouseMoveSelections<HeaderedItemsControl>();
        }

    }
}

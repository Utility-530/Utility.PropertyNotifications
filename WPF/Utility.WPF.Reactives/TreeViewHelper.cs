using System;
using System.Windows;
using System.Windows.Controls;

namespace Utility.WPF.Reactives
{
    public static class TreeViewHelper
    {
        /// <summary>
        /// <a href="https://social.msdn.microsoft.com/Forums/vstudio/en-US/c01df033-0acd-4690-a24d-3b7090694bc0/how-can-handle-treeviewitems-mouseclick-or-mousedoubleclick-event?forum=wpf"></a>
        /// Gets the top most item when the mouse double click returns multiple items, like with a TreeView
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static IObservable<TreeViewItem?> MouseDoubleClicks(this TreeView control)
        {
            return control
                    .MouseDoubleClickSelections<TreeViewItem>();
        }

        public static IObservable<TreeViewItem?> MouseSingleClicks(this TreeView control)
        {
            return control
                    .MouseSingleClickSelections<TreeViewItem>();
        }

        public static IObservable<TreeViewItem> MouseHoverEnters(this TreeView control)
        {
            return control
                    .MouseHoverEnterSelections<TreeViewItem>();
        }

        public static IObservable<TreeViewItem> MouseHoverLeaves(this TreeView control)
        {
            return control
                    .MouseHoverLeaveSelections<TreeViewItem>();
        }

        public static IObservable<(TreeViewItem item, Point point)> MouseMoves(this TreeView control)
        {
            return control
                    .MouseMoveSelections<TreeViewItem>();
        }

    }
}

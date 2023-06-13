using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Utility.WPF.Reactive
{
    public static class TreeViewHelper
    {
        /// <summary>
        /// <a href="https://social.msdn.microsoft.com/Forums/vstudio/en-US/c01df033-0acd-4690-a24d-3b7090694bc0/how-can-handle-treeviewitems-mouseclick-or-mousedoubleclick-event?forum=wpf"></a>
        /// Gets the top most item when the mouse double click returns multiple items, like with a TreeView
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static IObservable<TreeViewItem?> MouseDoubleClickTreeSelections(this TreeView control)
        {
            return control
                    .MouseDoubleClickSelections()
                    .Cast<TreeViewItem>();
        }
    }
}

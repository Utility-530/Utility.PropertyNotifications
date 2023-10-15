using System.Windows;
using System.Windows.Controls;

namespace Utility.Trees.Demo.Two
{
    public static class TreeViewHelper
    {
        public static void ClearSelections(this TreeView tview)
        {

            ClearTreeViewItemsControlSelection(tview.Items, tview.ItemContainerGenerator);

            static void ClearTreeViewItemsControlSelection(ItemCollection ic, ItemContainerGenerator icg)
            {
                for (int i = 0; i < ic.Count; i++)
                {
                    // Get the TreeViewItem
                    if (icg.ContainerFromIndex(i) is TreeViewItem tvi)
                    {
                        //Recursive call to traverse deeper levels
                        ClearTreeViewItemsControlSelection(tvi.Items, tvi.ItemContainerGenerator);
                        //Deselect the TreeViewItem 
                        tvi.IsSelected = false;
                    }
                }
            }
        }

        public static double FindDepth(this TreeView treeView, TreeViewItem treeViewItem)
        {
            var headerControl = GetHeaderControl(treeViewItem);
            Point pointA;

            if (headerControl == null)
            {
                pointA = new(0, treeViewItem.RenderSize.Height / 2d);
            }
            else
            {
                var x = headerControl.RenderSize;
                Point ofs = new(0, x.Height / 2d);
                pointA = headerControl.TransformToAncestor(treeViewItem).Transform(ofs);

            }

            return treeViewItem.TransformToAncestor(treeView).Transform(pointA).Y;
        }

        public static FrameworkElement? GetHeaderControl(TreeViewItem item)
        {
            return (FrameworkElement?)item?.Template.FindName("PART_Header", item);
        }
    }
}
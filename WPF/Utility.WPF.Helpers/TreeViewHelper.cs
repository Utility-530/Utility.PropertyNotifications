using System.Windows.Controls;

namespace Utility.WPF.Helpers
{
    public static class TreeViewHelper
    {

        public static T? FindRecursive<T>(this ItemsControl treeView, object instance) where T : Control
        {
            foreach (var item in treeView.Items)
            {
                if (item is T tItem)
                {
                    if (tItem.DataContext == instance)
                    {
                        return tItem;
                    }
                    else if (tItem is ItemsControl itemsControl)
                    {
                        if (FindRecursive<T>(itemsControl, instance) is T xx)
                            return xx;
                    }
                }
                else if (item == instance)
                {
                    if (treeView is TreeViewItem { IsExpanded: false } treeViewItem)
                    {
                        var isExpanded = treeViewItem.IsExpanded;
                        treeViewItem.IsExpanded = true;
                        treeViewItem.UpdateLayout();
                        treeViewItem.IsExpanded = isExpanded;
                    }
                    var container = treeView.ItemContainerGenerator.ContainerFromItem(item);
                    return container as T;
                }
                else if (treeView.ItemContainerGenerator.ContainerFromItem(item) is ItemsControl itemsControl)
                {
                    var find = FindRecursive<T>(itemsControl, instance);
                    if (find is T xx)
                        return xx;
                }
            }
            return null;
        }
    }
}



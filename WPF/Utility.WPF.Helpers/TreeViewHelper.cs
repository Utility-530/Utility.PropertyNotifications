using System;
using System.Collections.Specialized;
using System.Windows;
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
        public static void ExpandAll(this ItemsControl treeView)
        {
            if (treeView is TreeViewItem treeViewItem)
            {
                treeViewItem.IsExpanded = true;
                treeViewItem.UpdateLayout();
            }
            foreach (var item in treeView.Items)
            {
                if (item is ItemsControl itemsControl)
                {
                    ExpandAll(itemsControl);
                }
            }
        }

        public static TreeViewItem ContainerFromItemRecursive(this ItemContainerGenerator root, object item)
        {
            var treeViewItem = root.ContainerFromItem(item) as TreeViewItem;
            if (treeViewItem != null)
                return treeViewItem;
            foreach (var subItem in root.Items)
            {
                treeViewItem = root.ContainerFromItem(subItem) as TreeViewItem;
                var search = treeViewItem?.ItemContainerGenerator.ContainerFromItemRecursive(item);
                if (search != null)
                    return search;
            }
            return null;
        }


        public static void ExpandAllWithTracking(TreeViewItem treeView)
        {
            foreach (var item in treeView.Items)
            {
                if (treeView.ItemContainerGenerator.ContainerFromItem(item) is TreeViewItem tvi)
                {
                    ExpandWithTracking(tvi);
                }
            }
        }

        private static void ExpandWithTracking(TreeViewItem item)
        {
            item.IsExpanded = true;

            // Expand children that already exist
            item.UpdateLayout();

            HookCollectionChanged(item);

            for (int i = 0; i < item.Items.Count; i++)
            {
                if (item.ItemContainerGenerator.ContainerFromIndex(i) is TreeViewItem child)
                {
                    ExpandWithTracking(child);
                }
            }
        }

        private static void HookCollectionChanged(TreeViewItem item)
        {
            if (item.Items is INotifyCollectionChanged incc)
            {
                // Strong reference creates a memory leak → use weak handler
                EventHandler<NotifyCollectionChangedEventArgs> handler = null;

                handler = (s, e) =>
                {
                    // When new items are added, expand and recurse
                    if (e.NewItems != null)
                    {
                        item.UpdateLayout();

                        foreach (var newItem in e.NewItems)
                        {
                            var container = item.ItemContainerGenerator.ContainerFromItem(newItem) as TreeViewItem;

                            if (container != null)
                            {
                                ExpandWithTracking(container);
                            }
                        }
                    }
                };

                WeakEventManager<INotifyCollectionChanged, NotifyCollectionChangedEventArgs>
                    .AddHandler(incc, "CollectionChanged", handler);
            }
        }
    }
}
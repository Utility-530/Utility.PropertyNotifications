using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Windows.Controls;

namespace Utility.Trees.Demo
{
    public static class TreeHelper
    {
        public static T? FindRecursive<T>(ItemsControl treeView, object instance) where T : Control
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
                        treeViewItem.IsExpanded = true;
                        treeViewItem.UpdateLayout();
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

        public static IDisposable ExploreTree<T, TNode, TArgs>(T items, Func<T, TNode, T> funcAdd, Action<T, TNode> funcRemove, TNode property, Func<TNode, IObservable<TArgs>> funcChildren, Func<TArgs, IEnumerable<TNode>> newItems, Func<TArgs, IEnumerable<TNode>> oldItems)
        {

            items = funcAdd(items, property);

            var disposable = funcChildren(property)
                .Subscribe(item =>
                {
                    foreach (TNode node in newItems(item))
                    {
                        _ = ExploreTree(items, funcAdd, funcRemove, node, funcChildren, newItems, oldItems);
                    }
                    foreach (TNode node in oldItems(item))
                    {
                        funcRemove(items, node);
                        //_ = ExploreTree(items, func, node, state);
                    }
                },
                e =>
                {

                },
                () =>
                {
                }
              );
            return disposable;

        }
    }
}
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Media;

namespace Utility.WPF.Helpers
{
    public class TreeViewHelper
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
    }
}
//        public static ObservableCollection<TreeViewItem> Obser()
//        {
//            ObservableCollection<TreeViewItem> list = new();

//            subject.Subscribe(a =>
//            {
//                if (a.Action == NotifyCollectionChangedAction.Add)
//                {
//                    list.Add(a.NewItems.Cast<TreeViewItem>().Single());
//                }
//                if (a.Action == NotifyCollectionChangedAction.Remove)
//                {
//                    list.Remove(a.OldItems.Cast<TreeViewItem>().Single());
//                }
//            });



//        }

//        / <summary>
//        / Recursively search for an item in this subtree.
//        / </summary>
//        / <param name = "container" >
//        / The parent ItemsControl.This can be a TreeView or a TreeViewItem.
//        / </param>
//        / <param name = "item" >
//        / The item to search for.
//        / </param>
//        / <returns>
//        / The TreeViewItem that contains the specified item.
//        / </returns>
//        public static IEnumerable<TreeViewItem> TreeViewItems(ItemsControl container)
//        {
//            Observable.Create()
//            if (container != null)
//            {
//                yield break;
//            }
//            if (container.DataContext == item)
//            {
//                return container as TreeViewItem;
//            }

//            Expand the current container
//            if (container is TreeViewItem treeViewItem)
//            {
//                if (!treeViewItem.IsExpanded)
//                {
//                    container.SetValue(TreeViewItem.IsExpandedProperty, true);
//                }
//                yield return treeViewItem;

//            }
//            Try to generate the ItemsPresenter and the ItemsPanel.
//             by calling ApplyTemplate.Note that in the
//             virtualizing case even if the item is marked
//             expanded we still need to do this step in order to
//             regenerate the visuals because they may have been virtualized away.

//            container.ApplyTemplate();
//            ItemsPresenter itemsPresenter =
//                (ItemsPresenter)container.Template.FindName("ItemsHost", container);
//            if (itemsPresenter != null)
//            {
//                itemsPresenter.ApplyTemplate();
//            }
//            else
//            {
//                The Tree template has not named the ItemsPresenter,
//                so walk the descendents and find the child.
//               itemsPresenter = (container).ChildOfType<ItemsPresenter>();
//                if (itemsPresenter == null)
//                {
//                    container.UpdateLayout();

//                    itemsPresenter = (container).ChildOfType<ItemsPresenter>();
//                }
//            }

//            Panel itemsHostPanel = (Panel)VisualTreeHelper.GetChild(itemsPresenter, 0);

//            Ensure that the generator for this panel has been created.

//           UIElementCollection children = itemsHostPanel.Children;

//           VirtualizingStackPanel virtualizingPanel =
//               itemsHostPanel as VirtualizingStackPanel;

//            for (int i = 0, count = container.Items.Count; i < count; i++)
//            {
//                TreeViewItem subContainer;
//                if (virtualizingPanel != null)
//                {
//                    Bring the item into view so
//                     that the container will be generated.
//                    virtualizingPanel.GetType().GetMethod("BringIntoView").Invoke(virtualizingPanel, new object?[] { i });

//                    subContainer =
//                        (TreeViewItem)container.ItemContainerGenerator.
//                        ContainerFromIndex(i);
//                }
//                else
//                {
//                    subContainer =
//                        (TreeViewItem)container.ItemContainerGenerator.
//                        ContainerFromIndex(i);

//                    Bring the item into view to maintain the
//                     same behavior as with a virtualizing panel.
//                    subContainer.BringIntoView();
//                }

//                if (subContainer != null)
//                {
//                    subContainer.Expanded += TreeViewItem_Expanded;
//                    subContainer.Collapsed += TreeViewItem_Collapsed;

//                    Search the next level for the object.
//                    foreach (var _treeViewItem in TreeViewItems(subContainer))
//                        {
//                            yield return _treeViewItem;
//                        }
//                }
//            }

//        }
//    }

//}



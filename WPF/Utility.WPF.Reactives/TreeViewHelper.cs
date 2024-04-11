using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Utility.Changes;
using Utility.WPF.Helpers;

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

        public static IObservable<(TreeViewItem item, MouseEventArgs eventArgs)> MouseHoverLeaves(this TreeView control)
        {
            return control
                    .MouseHoverLeaveSelections<TreeViewItem>();
        }

        public static IObservable<(TreeViewItem item, Point point)> MouseMoves(this TreeView control)
        {
            return control
                    .MouseMoveSelections<TreeViewItem>();
        }




        /// <summary>
        /// Recursively search for an item in this subtree.
        /// </summary>
        /// <param name="container">
        /// The parent ItemsControl. This can be a TreeView or a TreeViewItem.
        /// </param>
        /// <param name="item">
        /// The item to search for.
        /// </param>
        /// <returns>
        /// The TreeViewItem that contains the specified item.
        /// </returns>
        public static IObservable<Change<TreeViewItem>> VisibleItems(ItemsControl container)
        {
            return Observable.Create<Change<TreeViewItem>>(observer =>
            {
                CompositeDisposable composite = new();
                if (container == null)
                {
                    return Disposable.Empty;
                }
                //if (container.DataContext == item)
                //{
                //    return container as TreeViewItem;
                //}

                // Expand the current container
                if (container is TreeViewItem treeViewItem)
                {
                    if (!treeViewItem.IsExpanded)
                    {
                        container.SetValue(TreeViewItem.IsExpandedProperty, true);
                    }
                    else
                        observer.OnNext(new(treeViewItem, Changes.Type.Add));

                }
                // Try to generate the ItemsPresenter and the ItemsPanel.
                // by calling ApplyTemplate.  Note that in the
                // virtualizing case even if the item is marked
                // expanded we still need to do this step in order to
                // regenerate the visuals because they may have been virtualized away.

                container.ApplyTemplate();
                ItemsPresenter itemsPresenter =
                    (ItemsPresenter)container.Template.FindName("ItemsHost", container);
                if (itemsPresenter != null)
                {
                    itemsPresenter.ApplyTemplate();
                }
                else
                {
                    // The Tree template has not named the ItemsPresenter,
                    // so walk the descendents and find the child.
                    itemsPresenter = (container).ChildOfType<ItemsPresenter>();
                    if (itemsPresenter == null)
                    {
                        container.UpdateLayout();

                        itemsPresenter = (container).ChildOfType<ItemsPresenter>();
                    }
                }

                Panel itemsHostPanel = (Panel)VisualTreeHelper.GetChild(itemsPresenter, 0);

                // Ensure that the generator for this panel has been created.
                UIElementCollection children = itemsHostPanel.Children;

                VirtualizingStackPanel virtualizingPanel =
                    itemsHostPanel as VirtualizingStackPanel;

                for (int i = 0, count = container.Items.Count; i < count; i++)
                {
                    TreeViewItem subContainer;
                    if (virtualizingPanel != null)
                    {
                        // Bring the item into view so
                        // that the container will be generated.
                        virtualizingPanel.GetType().GetMethod("BringIntoView").Invoke(virtualizingPanel, new object?[] { i });

                        subContainer =
                            (TreeViewItem)container.ItemContainerGenerator.
                            ContainerFromIndex(i);
                    }
                    else
                    {
                        subContainer =
                            (TreeViewItem)container.ItemContainerGenerator.
                            ContainerFromIndex(i);

                        // Bring the item into view to maintain the
                        // same behavior as with a virtualizing panel.
                        subContainer.BringIntoView();
                    }

                    if (subContainer != null)
                    {
                        Observable.FromEventPattern<RoutedEventHandler, RoutedEventArgs>(h => subContainer.Expanded += h, h => subContainer.Expanded -= h)
                        .Subscribe(a => observer.OnNext(new Change<TreeViewItem>(subContainer, Changes.Type.Add)))
                        .DisposeWith(composite);

                        Observable.FromEventPattern<RoutedEventHandler, RoutedEventArgs>(h => subContainer.Collapsed += h, h => subContainer.Collapsed -= h)
                        .Subscribe(a => observer.OnNext(new Change<TreeViewItem>(subContainer, Changes.Type.Remove)))
                        .DisposeWith(composite);

                        // Search the next level for the object.
                        VisibleItems(subContainer)
                        .Subscribe(change =>
                        {
                            observer.OnNext(change);
                        })
                        .DisposeWith(composite);
                    }
                }
                return composite;
            });

        }
    }

}


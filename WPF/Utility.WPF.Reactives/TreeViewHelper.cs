using System;
using System.Collections.Specialized;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Utility.Changes;
using Utility.Observables.Generic;
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

        public static IObservable<object> SelectedItemChanges(this TreeView selector)
        {
            return
                Observable
                .FromEventPattern<RoutedPropertyChangedEventHandler<object>, object>
                (a => selector.SelectedItemChanged += a, a => selector.SelectedItemChanged -= a)
                .StartWith(selector.SelectedItem)
                .Where(a => a != null);
        }

        public static IObservable<T> FindRecursiveAsync<T>(this ItemsControl treeView, object instance) where T : Control
        {
            return Observable.Create<T>(observer =>
            {
                CompositeDisposable disposables = [];

                if (treeView.ItemsSource is INotifyCollectionChanged notifyCollectionChanged)
                {
                    notifyCollectionChanged.CollectionChanged += (s, e) => update(e, observer, disposables);
                }
                foreach (var item in treeView.Items)
                {
                    match(treeView, instance, observer, disposables, item);
                }
                return disposables;
            });

            static void match<T>(ItemsControl treeView, object instance, IObserver<T> observer, CompositeDisposable disposables, object? item) where T : Control
            {
                if (item is T tItem)
                {
                    if (tItem.DataContext == instance)
                    {
                        observer.OnNext(tItem);
                    }
                    else if (tItem is ItemsControl itemsControl)
                    {
                        FindRecursiveAsync<T>(itemsControl, instance).Subscribe(a =>
                        {
                            observer.OnNext(a);
                        }).DisposeWith(disposables);
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
                    observer.OnNext(container as T);
                }
                else if (treeView.ItemContainerGenerator.ContainerFromItem(item) is ItemsControl itemsControl)
                {
                    FindRecursiveAsync<T>(itemsControl, instance)
                    .Subscribe(a =>
                    {
                        observer.OnNext(a);
                    })
                    .DisposeWith(disposables);
                }
            }

            void update(NotifyCollectionChangedEventArgs e, IObserver<T> observer, CompositeDisposable disposables)
            {
                foreach (var item in treeView.Items)
                {
                    if (item is TreeViewItem treeViewItem)
                    {
                        if (treeViewItem.IsLoaded)
                        {
                            match<T>(treeView, instance, observer, disposables, item);
                        }
                        else
                        {
                            treeViewItem.Loaded += TreeViewItem_Loaded;
                        }
                    }
                }

                void TreeViewItem_Loaded(object sender, RoutedEventArgs e)
                {
                    match<T>(treeView, instance, observer, disposables, sender);
                }
            }
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

                if (container.IsLoaded)
                    Container_Loaded(default, default);
                else
                    container.Loaded += Container_Loaded;

                void Container_Loaded(object sender, RoutedEventArgs e)
                {
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

                        treeViewItem.Expanded += TreeViewItem_Expanded;
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
                    Panel itemsHostPanel;

                    try
                    {
                        itemsHostPanel = (Panel)VisualTreeHelper.GetChild(itemsPresenter, 0);
                    }
                    catch (ArgumentOutOfRangeException ex)
                    {
                        //throw ex;
                        return;
                    }

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
                            subContainer?.BringIntoView();
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
                }
                return composite;
            });
        }

        private static void TreeViewItem_Expanded(object sender, RoutedEventArgs e)
        {
        }
    }
}
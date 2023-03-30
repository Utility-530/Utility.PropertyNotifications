using Jellyfish;
using MintPlayer.ObservableCollection;
using NetFabric.Hyperlinq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Reactive.Subjects;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Utility.WPF.Demo.Common.ViewModels;

namespace Utility.WPF.Demo.Trees
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class CustomStyleUserControl : UserControl
    {
        public CustomStyleUserControl()
        {
            InitializeComponent();
            this.Loaded += CustomStyleUserControl_Loaded;

        }

        private void CustomStyleUserControl_Loaded(object sender, RoutedEventArgs e)
        {
            dfsf();
        }

        void dfsf()
        {
            List<TreeViewItem> list = new();
            xasd.GetTreeViewItems(MyTreeView).ToArray();
            var foo = new Uri("/Utility.WPF.Controls.Trees;component/Themes/Generic.xaml", UriKind.RelativeOrAbsolute);
            var resourceDictionary = new ResourceDictionary() { Source = foo };
            var collection = new ObservableCollection<ButtonViewModel>();
            ItemsControl.ItemsSource = collection;

            foreach (var style in FindResourcesByType(resourceDictionary, typeof(TreeViewItem)).ToArray())
            {

                collection.Add(new ButtonViewModel
                {
                    Header = style.Key,
                    Command = new RelayCommand((a) =>
                    {
                        MyTreeView.ItemContainerStyleSelector = null;
                        TreeItemContainerStyleSelector.Instance.Current = style.Value;
                        MyTreeView.ItemContainerStyleSelector = TreeItemContainerStyleSelector.Instance;

                        foreach (var item in list)
                        {
                            item.ItemContainerStyleSelector = null;
                            item.ItemContainerStyleSelector = TreeItemContainerStyleSelector.Instance;
                        }
                    })
                });

            }

            xasd.Observable.Subscribe(a =>
            {
                if (a.Action == NotifyCollectionChangedAction.Add)
                {
                    list.Add(a.NewItems.Cast<TreeViewItem>().Single());
                }
                if (a.Action == NotifyCollectionChangedAction.Remove)
                {
                    list.Remove(a.OldItems.Cast<TreeViewItem>().Single());
                }
            });
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MyTreeView.ItemContainerStyleSelector = null;
            MyTreeView.ItemContainerStyleSelector = TreeItemContainerStyleSelector.Instance;
        }


        private IEnumerable<KeyValuePair<string?, Style>> FindResourcesByType(ResourceDictionary resources, Type type)
        {
            return resources.MergedDictionaries.SelectMany(d => FindResourcesByType(d, type)).Union(resources
                .Cast<DictionaryEntry>()
                .Where(s => s.Value is Style style && style.TargetType == type)
                .Select(a => new KeyValuePair<string?, Style>(a.Key?.ToString(), (Style)a.Value)));
        }
    }


    static class xasd
    {


        public static IEnumerable<TreeViewItem> EnumerateTreeViewItems(this Visual @this)
        {
            if (@this == null)
                yield break;

            var result = new List<TreeViewItem>();

            var frameworkElement = @this as FrameworkElement;
            if (frameworkElement != null)
            {
                frameworkElement.ApplyTemplate();
            }

            Visual child = null;
            for (int i = 0, count = VisualTreeHelper.GetChildrenCount(@this); i < count; i++)
            {
                child = VisualTreeHelper.GetChild(@this, i) as Visual;

                var treeViewItem = child as TreeViewItem;
                if (treeViewItem != null)
                {
                    yield return (treeViewItem);
                    treeViewItem.Expanded += TreeViewItem_Expanded;
                    treeViewItem.Collapsed += TreeViewItem_Collapsed;
                }
                foreach (var childTreeViewItem in EnumerateTreeViewItems(child))
                {
                    yield return (childTreeViewItem);
                }
            }
        }

        private static void TreeViewItem_Collapsed(object sender, RoutedEventArgs e)
        {
            if (sender is TreeViewItem treeViewItem)
            {
                subject.OnNext(new(NotifyCollectionChangedAction.Remove, treeViewItem));
            }
        }

        private static void TreeViewItem_Expanded(object sender, RoutedEventArgs e)
        {
            if (sender is TreeViewItem treeViewItem)
            {
                subject.OnNext(new(NotifyCollectionChangedAction.Add, treeViewItem));
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
        public static IEnumerable<TreeViewItem> GetTreeViewItems(ItemsControl container)
        {
            if (container != null)
            {
                //if (container.DataContext == item)
                //{
                //    return container as TreeViewItem;
                //}

                // Expand the current container
                if (container is TreeViewItem && !((TreeViewItem)container).IsExpanded)
                {
                    container.SetValue(TreeViewItem.IsExpandedProperty, true);
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
                    itemsPresenter = FindVisualChild<ItemsPresenter>(container);
                    if (itemsPresenter == null)
                    {
                        container.UpdateLayout();

                        itemsPresenter = FindVisualChild<ItemsPresenter>(container);
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
                        subContainer.Expanded += TreeViewItem_Expanded;

                        // Search the next level for the object.
                        foreach (var treeViewItem in GetTreeViewItems(subContainer))
                        {
                            yield return treeViewItem;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Search for an element of a certain type in the visual tree.
        /// </summary>
        /// <typeparam name="T">The type of element to find.</typeparam>
        /// <param name="visual">The parent element.</param>
        /// <returns></returns>
        private static T FindVisualChild<T>(Visual visual) where T : Visual
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(visual); i++)
            {
                Visual child = (Visual)VisualTreeHelper.GetChild(visual, i);
                if (child != null)
                {
                    T correctlyTyped = child as T;
                    if (correctlyTyped != null)
                    {
                        return correctlyTyped;
                    }

                    T descendent = FindVisualChild<T>(child);
                    if (descendent != null)
                    {
                        return descendent;
                    }
                }
            }

            return null;
        }

        static ReplaySubject<NotifyCollectionChangedEventArgs> subject = new ReplaySubject<NotifyCollectionChangedEventArgs>();

        public static IObservable<NotifyCollectionChangedEventArgs> Observable => subject;
    }

    public class TreeItemContainerStyleSelector : StyleSelector
    {
        public override Style SelectStyle(object item, DependencyObject container)
        {

            return Current;

            //return base.SelectStyle(item, container);
        }

        public Style Current { get; set; }

        public static TreeItemContainerStyleSelector Instance { get; } = new();
    }

}

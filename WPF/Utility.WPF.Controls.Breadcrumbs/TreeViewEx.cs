using System.Windows;
using System.Windows.Controls;
using Utility.WPF.Helpers;

namespace Utility.WPF.Controls.Breadcrumbs
{
    public class TreeViewEx
    {
        public static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.RegisterAttached(
                "SelectedItem",
                typeof(object),
                typeof(TreeViewEx),
                new UIPropertyMetadata(new object(), OnCurrentItemChanged));

        public static readonly DependencyProperty CurrentItemProperty =
            DependencyProperty.RegisterAttached(
                "CurrentItem",
                typeof(object),
                typeof(TreeViewEx),
                new UIPropertyMetadata(new object(), OnCurrentItemChanged));

        public static readonly DependencyProperty IsCollapseOnSelectionProperty =
            DependencyProperty.RegisterAttached(
                "IsCollapseOnSelection",
                typeof(bool),
                typeof(TreeViewEx),
                new UIPropertyMetadata(false));

        public static object GetSelectedItem(ItemsControl itemsControl)
        {
            return (object)itemsControl.GetValue(SelectedItemProperty);
        }

        public static void SetSelectedItem(ItemsControl itemsControl, object value)
        {
            itemsControl.SetValue(SelectedItemProperty, value);
        }

        public static object GetCurrentItem(ItemsControl CurrentItemBlock)
        {
            return (object)CurrentItemBlock.GetValue(CurrentItemProperty);
        }

        public static void SetCurrentItem(ItemsControl CurrentItemBlock, object value)
        {
            CurrentItemBlock.SetValue(CurrentItemProperty, value);
        }

        public static bool GetIsCollapseOnSelection(ItemsControl itemsControl)
        {
            return (bool)itemsControl.GetValue(IsCollapseOnSelectionProperty);
        }

        public static void SetIsCollapseOnSelection(ItemsControl itemsControl, bool value)
        {
            itemsControl.SetValue(IsCollapseOnSelectionProperty, value);
        }

        private static bool flag;
        private static Dictionary<TreeViewItem, TreeView> treeViews = [];

        private static void OnCurrentItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TreeView _treeView)
            {
                _treeView.SelectedItemChanged += SelectedItemChanged;
            }
            else if (d is TreeViewItem treeViewItem && TreeViewEx.treeViews.ContainsKey(treeViewItem) == false)
            {
                if (_toTreeView(treeViewItem) is TreeView treeView)
                {
                    treeView.SelectedItemChanged += (s, e) => SelectedItemChanged(treeViewItem, treeView, e);
                }
                else
                {
                    // view has potentially changed
                }
            }
            else if (d is TreeViewItem tvi && e.NewValue == null)
            {
                SetSelectedItem(tvi, null);
            }

            static TreeView? _toTreeView(TreeViewItem treeViewItem)
            {
                if (!treeViews.TryGetValue(treeViewItem, out TreeView? treeView))
                {
                    treeView = treeViewItem.FindParent<TreeView>();
                    treeViews[treeViewItem] = treeView;
                }

                return treeView;
            }
        }

        private static void SelectedItemChanged(TreeViewItem sender, TreeView treeView, RoutedPropertyChangedEventArgs<object> e)
        {
            if (flag == true)
                return;

            if (sender.DataContext.Equals(e.NewValue) == false)
            {
                if (sender.FindRecursive<TreeViewItem>(e.NewValue) is not TreeViewItem find)
                {
                    return;
                }
                else
                {
                    TreeViewItem child = find;

                    while (child != null && child.FindParent<TreeViewItem>() != sender)
                    {
                        child = child.FindParent<TreeViewItem>();
                    }
                    if (child != null)
                    {
                        SetSelectedItem(sender, child.DataContext);
                    }
                    else
                    {
                        //SetSelectedItem(sender, find.DataContext);
                    }
                }

                SetCurrentItem(sender, e.NewValue);
            }

            flag = true;

            //flag = false;
            //return;
            try
            {
                if (GetIsCollapseOnSelection(sender))
                {
                }
                //foreach (var item in (sender).Items.Cast<object>().ToArray())
                //{
                //    if (item is TreeViewItem citem)
                //    {
                //        citem.IsExpanded = false;
                //    }
                //    else if ((sender).ItemContainerGenerator.ContainerFromItem(item) is TreeViewItem _item)
                //    {
                //        _item.IsExpanded = false;
                //    }
                //}
            }
            finally
            {
                flag = false;
            }
        }

        private static void SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (flag == true)
                //return;
                flag = true;
            //CurrentNode = e.NewValue;
            SetCurrentItem((sender as TreeView), e.NewValue);
            //flag = false;
            //return;
            try
            {
                foreach (var item in (sender as TreeView).Items.Cast<object>().ToArray())
                {
                    if (item is TreeViewItem citem)
                    {
                        citem.IsExpanded = false;
                    }
                    else if ((sender as TreeView).ItemContainerGenerator.ContainerFromItem(item) is TreeViewItem _item)
                    {
                        _item.IsExpanded = false;
                    }
                }
            }
            finally
            {
                flag = false;
            }
        }
    }
}
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Linq;
using Utility.WPF.Helpers;

namespace Utility.WPF.Attached
{
    public static class TreeEx
    {

        public static readonly DependencyProperty IsChildSelectedProperty = DependencyProperty.RegisterAttached("IsChildSelected", typeof(bool), typeof(TreeEx), new PropertyMetadata(true, changed));
        public static readonly DependencyProperty SelectedContainerProperty = DependencyProperty.RegisterAttached("SelectedContainer", typeof(TreeViewItem), typeof(TreeEx), new PropertyMetadata());
        public static readonly DependencyProperty SelectedItemProperty = DependencyProperty.RegisterAttached("SelectedItem", typeof(object), typeof(TreeEx), new PropertyMetadata());
        public static readonly DependencyProperty IsRootNodeProperty = DependencyProperty.RegisterAttached("IsRootNode", typeof(bool), typeof(TreeEx), new UIPropertyMetadata(false));

        public static void SetIsChildSelected(DependencyObject element, bool value) => element.SetValue(IsChildSelectedProperty, value);
        public static bool GetIsChildSelected(DependencyObject element) => (bool)element.GetValue(IsChildSelectedProperty);
        public static void SetSelectedContainer(DependencyObject element, TreeViewItem value) => element.SetValue(SelectedContainerProperty, value);
        public static TreeViewItem GetSelectedContainer(DependencyObject element) => (TreeViewItem)element.GetValue(SelectedContainerProperty);
        public static void SetSelectedItem(DependencyObject element, object value) => element.SetValue(SelectedItemProperty, value);
        public static object GetSelectedItem(DependencyObject element) => element.GetValue(SelectedItemProperty);
        public static bool GetIsRootNode(DependencyObject obj) => (bool)obj.GetValue(IsRootNodeProperty);
        public static void SetIsRootNode(DependencyObject obj, bool value) => obj.SetValue(IsRootNodeProperty, value);

        private static void changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TreeViewItem item && item.FindParent<TreeView>() is TreeView treeView)
            {
                treeView.SelectedItemChanged += (s, e) =>
                {
                    var selectedContainer = treeView.ItemContainerGenerator.ContainerFromItemRecursive(treeView.SelectedItem);
                    if (TreeEx.GetIsRootNode(selectedContainer ))
                    {
                        e.Handled = true;
                        if(e.OldValue is TreeViewItem tvi)
                            tvi.IsSelected = true;
                        else if(e.OldValue is not null)
                        {
                            if(treeView.ItemContainerGenerator.ContainerFromItemRecursive(e.OldValue) is TreeViewItem _tvi)
                                _tvi.IsSelected = true;

                        }
                        return;
                    }

              
                    SetSelectedContainer(d, selectedContainer);
                    SetSelectedItem(d, treeView.SelectedItem);
                    SetIsChildSelected(item, item.FindChild<TreeViewItem>(t => selectedContainer == t) is TreeViewItem);
                };
            }
        }
    }
}

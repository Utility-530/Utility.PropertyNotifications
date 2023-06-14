using System.Windows.Controls;

namespace Utility.WPF.Controls
{
    public class TreeViewFirst : TreeView
    {
        protected override void OnItemsChanged(System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            bool b = false;
            foreach (object item in base.Items)
            {
                if (b) break;
                TreeViewItem treeItem = base.ItemContainerGenerator.ContainerFromItem(item) as TreeViewItem;
                if (treeItem != null)
                {
                    ExpandAll(treeItem, true);
                    treeItem.IsExpanded = true; b = true;
                }
            }
        }

        private void ExpandAll(ItemsControl items, bool expand)
        {
            bool b = false;
            foreach (object obj in items.Items)
            {
                if (b) break;
                ItemsControl childControl = items.ItemContainerGenerator.ContainerFromItem(obj) as ItemsControl;
                if (childControl != null)
                {
                    ExpandAll(childControl, expand);
                }
                TreeViewItem item = childControl as TreeViewItem;
                if (item != null)
                {
                    item.IsExpanded = true;
                    item.IsSelected = true;
                }
                b = true;
            }
        }
    }
}
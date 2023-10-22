using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;

namespace Utility.WPF.Helpers
{
    public class TreeViewHelper
    {


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
        public static IEnumerable<TreeViewItem> TreeViewItems(ItemsControl container)
        {
            if (container != null)
            {
                yield break;
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
                yield return treeViewItem;

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
                    //subContainer.Expanded += TreeViewItem_Expanded;
                    //subContainer.Collapsed += TreeViewItem_Collapsed;

                    // Search the next level for the object.
                    foreach (var _treeViewItem in TreeViewItems(subContainer))
                    {
                        yield return _treeViewItem;
                    }
                }
            }

        }
    }

}



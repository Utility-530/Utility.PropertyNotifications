using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Utility.Helpers.NonGeneric;

namespace Utility.WPF.Controls.Trees
{
    /// <summary>
    /// Based on AnimatedListBox
    /// </summary>
    public partial class CustomTreeViewItem
    {
        public static readonly DependencyProperty ScrollToSelectedItemProperty =
            DependencyProperty.Register("ScrollToSelectedItem", typeof(bool), typeof(CustomTreeViewItem),
                new PropertyMetadata(false));

        public static readonly DependencyProperty SelectedIndexOffsetProperty =
            DependencyProperty.Register("SelectedIndexOffset", typeof(int), typeof(CustomTreeViewItem),
              new PropertyMetadata(0));

        #region PART holders

        public static readonly DependencyProperty SelectedIndexProperty = Selector.SelectedIndexProperty.AddOwner(typeof(CustomTreeViewItem));

        private AnimatedScrollViewer.AnimatedScrollViewer _scrollViewer;

        #endregion PART holders

        private void animated_onApplyTemplate()
        {
            base.OnApplyTemplate();

            AnimatedScrollViewer.AnimatedScrollViewer scrollViewerHolder = base.GetTemplateChild("PART_AnimatedScrollViewer") as AnimatedScrollViewer.AnimatedScrollViewer;
            if (scrollViewerHolder != null)
            {
                _scrollViewer = scrollViewerHolder;
            }

            this.SelectionChanged += new SelectionChangedEventHandler(AnimatedListBox_SelectionChanged);
            this.Loaded += new RoutedEventHandler(AnimatedListBox_Loaded);
            this.LayoutUpdated += new EventHandler(AnimatedListBox_LayoutUpdated);
        }

        private void AnimatedListBox_LayoutUpdated(object sender, EventArgs e)
        {
            updateScrollPosition(sender);
        }

        private void AnimatedListBox_Loaded(object sender, RoutedEventArgs e)
        {
            updateScrollPosition(sender);
        }

        private void AnimatedListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.ItemsSource == null)
                return;
            var index = this.ItemsSource.IndexOf(a => a == e.AddedItems.Cast<object>().FirstOrDefault());
            if (sender is CustomTreeViewItem item)
            {
                item.SelectedIndex = index;
            }
            updateScrollPosition(sender);
        }

        public void updateScrollPosition(object sender)
        {
            CustomTreeViewItem thisLB = (CustomTreeViewItem)sender;

            if (thisLB != null)
            {
                if (thisLB.ScrollToSelectedItem)
                {
                    double scrollTo = 0;
                    for (int i = 0; i < (thisLB.SelectedIndex + thisLB.SelectedIndexOffset); i++)
                    {
                        TreeViewItem tempItem = thisLB.ItemContainerGenerator.ContainerFromItem(thisLB.Items[i]) as TreeViewItem;

                        if (tempItem != null)
                        {
                            scrollTo += tempItem.ActualHeight;
                        }
                    }

                    _scrollViewer.TargetVerticalOffset = scrollTo;
                }
            }
        }

        #region ScrollToSelectedItem (DependencyProperty)

        public int SelectedIndex
        {
            get { return (int)GetValue(SelectedIndexProperty); }
            set { SetValue(SelectedIndexProperty, value); }
        }

        /// <summary>
        /// A description of the property.
        /// </summary>
        public bool ScrollToSelectedItem
        {
            get { return (bool)GetValue(ScrollToSelectedItemProperty); }
            set { SetValue(ScrollToSelectedItemProperty, value); }
        }

        #endregion ScrollToSelectedItem (DependencyProperty)

        #region SelectedIndexOffset (DependencyProperty)

        /// <summary>
        /// Use this property to choose the scroll to an item that is not selected, but is X above or below the selected item
        /// </summary>
        public int SelectedIndexOffset
        {
            get { return (int)GetValue(SelectedIndexOffsetProperty); }
            set { SetValue(SelectedIndexOffsetProperty, value); }
        }

        #endregion SelectedIndexOffset (DependencyProperty)
    }
}
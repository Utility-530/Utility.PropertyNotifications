using Microsoft.Xaml.Behaviors;
using System;
using System.Linq;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Utility.WPF.Helpers;
using Utility.WPF.Reactives;

namespace Utility.WPF.Behaviors
{
    /// <summary>
    /// Returns the indices of rows in the DataGrid that are visible to the user
    /// </summary>
    public class ItemsControlVisibleItemsBehavior : Behavior<ItemsControl>
    {
        private double scrollPosition;

        public static readonly DependencyProperty FirstIndexProperty = DependencyProperty.Register("FirstIndex", typeof(int), typeof(ItemsControlVisibleItemsBehavior), new PropertyMetadata(0));
        public static readonly DependencyProperty LastIndexProperty = DependencyProperty.Register("LastIndex", typeof(int), typeof(ItemsControlVisibleItemsBehavior), new PropertyMetadata(0));
        public static readonly DependencyProperty SizeProperty = DependencyProperty.Register("Size", typeof(int), typeof(ItemsControlVisibleItemsBehavior), new PropertyMetadata(0));
        public static readonly DependencyProperty MouseFactorProperty = DependencyProperty.Register("MouseFactor", typeof(int), typeof(ItemsControlVisibleItemsBehavior), new PropertyMetadata(3));

        public int FirstIndex
        {
            get { return (int)GetValue(FirstIndexProperty); }
            set { SetValue(FirstIndexProperty, value); }
        }

        public int LastIndex
        {
            get { return (int)GetValue(LastIndexProperty); }
            set { SetValue(LastIndexProperty, value); }
        }

        public int Size
        {
            get { return (int)GetValue(SizeProperty); }
            set { SetValue(SizeProperty, value); }
        }

        public int MouseFactor
        {
            get { return (int)GetValue(MouseFactorProperty); }
            set { SetValue(MouseFactorProperty, value); }
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.Loaded += (_, _) => DataGridIsLoadedChanges();
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();

            if (this.AssociatedObject != null)
            {
                AssociatedObject.Loaded -= (_, _) => DataGridIsLoadedChanges();
            }
        }

        private void DataGridIsLoadedChanges()
        {
            if (VisualTreeExHelper.FindVisualChildren<ScrollViewer>(AssociatedObject).FirstOrDefault() is ScrollViewer scrollViewer)
            {
                // N.B this doesn't work well if VerticalScrollBar is used to scroll- works for mouse-wheel.
                if (MouseFactor > 1)
                    scrollViewer.ScrollChanged += AssociatedObject_ScrollChanged;

                if (VisualTreeExHelper.FindVisualChildren<ScrollBar>(scrollViewer).Single(s => s.Orientation == Orientation.Vertical) is ScrollBar verticalScrollBar)

                    scrollViewer
                        .Changes()

                        .Select(a => ScrollViewerOnScrollChanged(verticalScrollBar, AssociatedObject, a))
                        .Where(a => a.HasValue)
                        .Select(a => a!.Value)
                        .Subscribe(a =>
                    {
                        var (firstVisible, lastVisible) = a;
                        FirstIndex = firstVisible;
                        LastIndex = lastVisible; 

                        Size = lastVisible - firstVisible + 1;
                    });
                else
                    throw new Exception("fgl; 4gsdg");
            }

            void AssociatedObject_ScrollChanged(object sender, ScrollChangedEventArgs e)
            {
                if (scrollViewer.VerticalOffset == scrollPosition)
                    return;

                scrollPosition = scrollViewer.VerticalOffset + e.VerticalChange * MouseFactor;
                if (scrollPosition >= scrollViewer.ScrollableHeight)
                    scrollViewer.ScrollToBottom();
                else
                    scrollViewer.ScrollToVerticalOffset(scrollPosition);
            }

            static (int, int)? ScrollViewerOnScrollChanged(ScrollBar verticalScrollBar, ItemsControl dataGrid, ScrollChangedEventArgs a)
            {
                int totalCount = dataGrid.Items.Count;
                var firstVisible = verticalScrollBar.Value;
                var lastVisible = firstVisible + totalCount - verticalScrollBar.Maximum;

                return ((int)firstVisible, (int)lastVisible);
            }
        }
    }
}
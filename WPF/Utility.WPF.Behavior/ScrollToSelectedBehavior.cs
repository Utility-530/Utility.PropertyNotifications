//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.Windows.Controls;
//using System.Windows;
//using System.Windows.Controls.Primitives;
//using Microsoft.Xaml.Behaviors;

//namespace Utility.WPF.Behavior
//{
//    public class ScrollToSelectedBehavior : Behavior<ListBox>
//    {
//        protected override void OnAttached()
//        {
//            AssociatedObject.SelectionChanged += SelectionChanged;
//            base.OnAttached();
//        }

//        protected override void OnDetaching()
//        {
//            AssociatedObject.SelectionChanged -= SelectionChanged;
//            base.OnDetaching();
//        }


//        private async void SelectionChanged(object sender, SelectionChangedEventArgs selectionChangedEventArgs)
//        {
//            if (AssociatedObject.SelectedItem == null)
//            {
//                return;
//            }

//            var item = AssociatedObject.SelectedItem;

//            // Calculations relative to screen or ListView
//            var listItem = (FrameworkElement)AssociatedObject.ItemContainerGenerator.ContainerFromItem(item);

//            if (listItem == null)
//            {
//                AssociatedObject.ScrollIntoView(item);
//            }

//            while (listItem == null)
//            {
//                await Task.Delay(1); // wait for scrolling to complete - it takes a moment
//                listItem = (FrameworkElement)AssociatedObject.ItemContainerGenerator.ContainerFromItem(item);
//            }


//            var index = (int)AssociatedObject.ItemContainerGenerator.IndexFromContainer(listItem);

//            var count = AssociatedObject.Items.Count;

//            var scrollViewer = AssociatedObject.FindChild<ScrollViewer>();

//            scrollViewer.ScrollToVerticalOffset((index * 1.0d * scrollViewer.ScrollableHeight) / count);
//            //var topLeft = GetElementPosition(listItem, AssociatedObject).Y;
//            //var lvih = listItem.ActualHeight;
//            //var lvh = AssociatedObject.ActualHeight;
//            //var desiredTopLeft = (lvh - lvih) / 2.0;
//            //var desiredDelta = topLeft - desiredTopLeft;

//            //// Calculations relative to the ScrollViewer within the ListView
//            //var scrollViewer = AssociatedObject.FindChild<ScrollViewer>();
//            //var currentOffset = scrollViewer.VerticalOffset;
//            //var desiredOffset = currentOffset + desiredDelta;
//            //var scaleddesiredOffset = desiredOffset / scrollViewer.VerticalOffset;
//            //scrollViewer.ScrollToVerticalOffset(scaleddesiredOffset);

//            // better yet if building for Windows 8.1 to make the scrolling smoother use:
//            // scrollViewer.ChangeView(null, desiredOffset, null);
//        }


//        Point GetElementPosition(FrameworkElement childElement, FrameworkElement absoluteElement)
//        {
//            return childElement.TransformToAncestor(absoluteElement).Transform(new Point(0, 0));
//        }

//    }
//}
